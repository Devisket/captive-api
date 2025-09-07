using Captive.Model.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Captive.Orchestrator.Services.Barcode.Implementations
{
    public class MTBCBarcodeService : IBarcodeImplementationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MTBCBarcodeService> _logger;

        public MTBCBarcodeService(IConfiguration configuration, ILogger<MTBCBarcodeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string BarcodeServiceName => "MTBCBarcode";

        public async Task<IEnumerable<UpdateCheckOrderBarcodeDto>> GenerateBarcode(Guid bankId, Guid batchId, IEnumerable<CheckOrderBarcodeDto> checkOrders)
        {
            var cliPath = _configuration["BarcodeService:Mtbc"];
            
            if (string.IsNullOrEmpty(cliPath))
            {
                _logger.LogError("BarcodeService:Mtbc configuration is missing or empty");
                throw new InvalidOperationException("BarcodeService:Mtbc configuration is missing or empty");
            }

            if (!File.Exists(cliPath))
            {
                _logger.LogError($"BarcodeGenerator CLI not found at path: {cliPath}");
                throw new FileNotFoundException($"BarcodeGenerator CLI not found at path: {cliPath}");
            }

            var updateBarcodeRequests = new List<UpdateCheckOrderBarcodeDto>();

            foreach (var checkOrder in checkOrders)
            {
                _logger.LogInformation($"Processing barcode generation for CheckOrderId: {checkOrder.CheckOrderId}");

                // Process each check series in the check order
                foreach (var checkSeries in checkOrder.CheckInventories.OrderByDescending(x => x.StartingSeries))
                {
                    // Generate all series between starting and ending series as semicolon-separated string
                    var seriesString = GenerateSeriesBetween(checkSeries.StartingSeries, checkSeries.EndingSeries);

                    try
                    {
                        var barcodeValue = await GenerateBarcodeForSeries(cliPath, checkOrder.AccountNumber, checkOrder.BRSTN, seriesString);

                        updateBarcodeRequests.Add(new UpdateCheckOrderBarcodeDto
                        {
                            CheckOrderId = checkOrder.CheckOrderId,
                            BarcodeValue = barcodeValue,
                            CheckInventoryDetailId = checkSeries.CheckInventoryDetailId,
                        });


                        _logger.LogInformation($"Successfully generated barcode for CheckOrderId: {checkOrder.CheckOrderId}, Series: {seriesString}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to generate barcode for CheckOrderId: {checkOrder.CheckOrderId}, Series: {seriesString}");
                        throw;
                    }
                }
            }
            return updateBarcodeRequests;
        }

        /// <summary>
        /// Generates a semicolon-separated string of all series between starting and ending series (inclusive)
        /// Example: StartingSeries="ABC000001", EndingSeries="ABC000010" -> "ABC000001;ABC000002;ABC000003;...;ABC000010"
        /// </summary>
        private string  GenerateSeriesBetween(string startingSeries, string endingSeries)
        {
            var result = new List<string>();
            
            if (string.IsNullOrEmpty(startingSeries) || string.IsNullOrEmpty(endingSeries))
            {
                return string.Empty;
            }

            // Find where the numeric part starts from the end for both series
            int startNumericIndex = FindNumericStartIndex(startingSeries);
            int endNumericIndex = FindNumericStartIndex(endingSeries);

            // Extract prefix and numeric parts for starting series
            string startPrefix = startingSeries.Substring(0, startNumericIndex);
            string startNumericPart = startingSeries.Substring(startNumericIndex);
            
            // Extract prefix and numeric parts for ending series
            string endPrefix = endingSeries.Substring(0, endNumericIndex);
            string endNumericPart = endingSeries.Substring(endNumericIndex);

            // Validate that prefixes match
            if (startPrefix != endPrefix)
            {
                _logger.LogWarning($"Series prefixes don't match: '{startPrefix}' vs '{endPrefix}'. Using starting series only.");
                return startingSeries;
            }

            // Parse numeric parts
            if (!long.TryParse(startNumericPart, out long startNumber) || !long.TryParse(endNumericPart, out long endNumber))
            {
                _logger.LogWarning($"Failed to parse numeric parts: '{startNumericPart}' or '{endNumericPart}'. Using starting series only.");
                return startingSeries;
            }

            // Validate that start <= end
            if (startNumber > endNumber)
            {
                _logger.LogWarning($"Starting number {startNumber} is greater than ending number {endNumber}. Using starting series only.");
                return startingSeries;
            }

            // Use the padding length from the starting series
            int paddingLength = startNumericPart.Length;

            // Generate all series from start to end (inclusive)
            for (long currentNumber = endNumber; currentNumber >= startNumber; currentNumber--)
            {
                string formattedNumber = currentNumber.ToString().PadLeft(paddingLength, '0');
                result.Add(startPrefix + formattedNumber);
            }

            // Join all series with semicolons
            return string.Join(";", result);
        }

        /// <summary>
        /// Finds the index where the numeric part starts from the end of the string
        /// </summary>
        private int FindNumericStartIndex(string series)
        {
            int numericStartIndex = series.Length;
            for (int i = series.Length - 1; i >= 0; i--)
            { 
                var c = series[i];
                if (!char.IsDigit(c))
                {
                    numericStartIndex = i + 1;
                    break;
                }
                if (i == 0) // All characters from start are digits
                {
                    numericStartIndex = 0;
                }
            }
            return numericStartIndex;
        }

        private async Task<string> GenerateBarcodeForSeries(string cliPath, string accountNumber, string brstn, string series)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = cliPath,
                Arguments = $"\"{accountNumber}\" \"{brstn}\" \"{series}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _logger.LogInformation($"Executing CLI: {cliPath} with arguments: {startInfo.Arguments}");

            using var process = new Process();
            process.StartInfo = startInfo;
            
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
            {
                _logger.LogError($"CLI execution failed with exit code {process.ExitCode}. Error: {error}");
                throw new InvalidOperationException($"BarcodeGenerator CLI failed with exit code {process.ExitCode}. Error: {error}");
            }
            
            if (!string.IsNullOrEmpty(output))
            {
                _logger.LogInformation($"CLI output: {output}");
                // Return the generated barcode value (assuming CLI outputs the barcode value)
                return output.Trim();
            }

            // If no output, return a placeholder or handle accordingly
            return $"BARCODE_{accountNumber}_{brstn}_{series}";
        }
    }
}
