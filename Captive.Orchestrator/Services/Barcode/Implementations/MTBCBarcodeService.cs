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
        private const int MAX_RETRY_ATTEMPTS = 3;
        private const int RETRY_DELAY_MS = 100;

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


            _logger.LogInformation($"Starting To Generate Barcode");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var checkOrder in checkOrders)
            {
                _logger.LogInformation($"Processing barcode generation for CheckOrderId: {checkOrder.CheckOrderId}");

                // Create tasks for all series in parallel
                var barcodeTasks = checkOrder.CheckInventories
                    .OrderByDescending(x => x.StartingSeries)
                    .Select(async checkSeries =>
                    {
                        var seriesString = GenerateSeriesBetween(checkSeries.StartingSeries, checkSeries.EndingSeries);

                        try
                        {
                            var barcodeValue = await GenerateBarcodeForSeries(cliPath, checkOrder.AccountNumber, checkOrder.BRSTN, seriesString);

                            _logger.LogInformation($"Successfully generated barcode for CheckOrderId: {checkOrder.CheckOrderId}, Series: {seriesString}");

                            return new UpdateCheckOrderBarcodeDto
                            {
                                CheckOrderId = checkOrder.CheckOrderId,
                                BarcodeValue = barcodeValue,
                                CheckInventoryDetailId = checkSeries.CheckInventoryDetailId,
                            };
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to generate barcode for CheckOrderId: {checkOrder.CheckOrderId}, Series: {seriesString}");
                            throw;
                        }
                    })
                    .ToList();

                // Wait for all parallel operations to complete
                var results = await Task.WhenAll(barcodeTasks);
                updateBarcodeRequests.AddRange(results);
            }

            stopWatch.Stop();

            _logger.LogInformation($"Generate Barcode Finished with {stopWatch.Elapsed.TotalSeconds} seconds");

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
            Exception lastException = null;

            for (int attempt = 1; attempt <= MAX_RETRY_ATTEMPTS; attempt++)
            {
                try
                {
                    var result = await ExecuteCliProcess(cliPath, accountNumber, brstn, series, attempt);

                    // Success! Log if we had to retry
                    if (attempt > 1)
                    {
                        _logger.LogInformation($"Successfully generated barcode on attempt {attempt} for AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");
                    }

                    return result;
                }
                catch (InvalidOperationException ex)
                {
                    lastException = ex;

                    // Check if this is an access violation error
                    bool isAccessViolation = ex.Message.Contains("access violation", StringComparison.OrdinalIgnoreCase) ||
                                            ex.Message.Contains("-1073741819");

                    if (isAccessViolation && attempt < MAX_RETRY_ATTEMPTS)
                    {
                        var delay = RETRY_DELAY_MS * attempt; // Exponential backoff
                        _logger.LogWarning($"Access violation detected on attempt {attempt}/{MAX_RETRY_ATTEMPTS}. Retrying after {delay}ms... AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");
                        await Task.Delay(delay);

                        // Force garbage collection to clean up any orphaned resources
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    else
                    {
                        // Either not an access violation, or we've exhausted retries
                        throw;
                    }
                }
            }

            // All retry attempts failed
            _logger.LogError($"All {MAX_RETRY_ATTEMPTS} attempts failed for AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}. Last error: {lastException?.Message}");
            throw new InvalidOperationException(
                $"Failed to generate barcode after {MAX_RETRY_ATTEMPTS} attempts. AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}",
                lastException);
        }

        private async Task<string> ExecuteCliProcess(string cliPath, string accountNumber, string brstn, string series, int attempt)
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

            _logger.LogInformation($"Executing CLI (attempt {attempt}): {cliPath} with arguments: AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");

            using var process = new Process();
            process.StartInfo = startInfo;

            var processStartTime = DateTime.UtcNow;
            process.Start();

            // Read both output and error streams concurrently to prevent deadlock
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);
            await process.WaitForExitAsync();

            var output = outputTask.Result;
            var error = errorTask.Result;
            var processEndTime = DateTime.UtcNow;
            var duration = (processEndTime - processStartTime).TotalMilliseconds;

            _logger.LogInformation($"CLI process completed in {duration}ms with exit code {process.ExitCode}");

            // Log stderr output even if exit code is 0 (might contain warnings)
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning($"CLI stderr output: {error}");
            }

            // Handle specific exit codes
            if (process.ExitCode == -1073741819) // 0xC0000005 - Access Violation
            {
                _logger.LogError($"CLI crashed with ACCESS_VIOLATION (exit code -1073741819) for AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}. Output: {output}, Error: {error}");
                throw new InvalidOperationException($"BarcodeGenerator CLI crashed with access violation -1073741819. AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");
            }

            if (process.ExitCode != 0)
            {
                _logger.LogError($"CLI execution failed with exit code {process.ExitCode} for AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}. Output: {output}, Error: {error}");
                throw new InvalidOperationException($"BarcodeGenerator CLI failed with exit code {process.ExitCode}. Error: {error}. AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");
            }

            if (!string.IsNullOrEmpty(output))
            {
                _logger.LogInformation($"CLI generated barcode successfully: {output}");
                return output.Trim();
            }

            _logger.LogWarning($"CLI returned exit code 0 but produced no output for AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");
            throw new InvalidOperationException($"BarcodeGenerator CLI produced no output. AccountNumber={accountNumber}, BRSTN={brstn}, Series={series}");
        }
    }
}
