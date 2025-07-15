using Captive.Model.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Captive.Fileprocessor.Services.Barcode.Implementations
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
                
                var barcodeValues = new List<string>();
                
                // Iterate through each starting series for this check order
                foreach (var startingSeries in checkOrder.StartingSeries)
                {
                    try
                    {
                        var barcodeValue = await GenerateBarcodeForSeries(cliPath, checkOrder.AccountNumber, checkOrder.BRSTN, startingSeries);
                        barcodeValues.Add(barcodeValue);
                        _logger.LogInformation($"Successfully generated barcode for CheckOrderId: {checkOrder.CheckOrderId}, StartingSeries: {startingSeries}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to generate barcode for CheckOrderId: {checkOrder.CheckOrderId}, StartingSeries: {startingSeries}");
                        throw;
                    }
                }

                // Combine all barcode values for this check order (you might want to adjust this logic)
                var combinedBarcodeValue = string.Join(";", barcodeValues);
                
                updateBarcodeRequests.Add(new UpdateCheckOrderBarcodeDto
                {
                    CheckOrderId = checkOrder.CheckOrderId,
                    BarcodeValue = combinedBarcodeValue
                });
            }


            return updateBarcodeRequests;
        }

        private async Task<string> GenerateBarcodeForSeries(string cliPath, string accountNumber, string brstn, string startingSeries)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = cliPath,
                Arguments = $"\"{accountNumber}\" \"{brstn}\" \"{startingSeries}\"",
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
            return $"BARCODE_{accountNumber}_{brstn}_{startingSeries}";
        }
    }
}
