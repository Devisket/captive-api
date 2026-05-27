using Captive.Orchestrator.Services.Barcode;
using Captive.Model.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Captive.Orchestrator.Services.GenerateBarcodeService
{
    public class GenerateBarcodeService : IGenerateBarcodeService
    {
        private readonly IBarcodeImplementationFactory _barcodeFactory;

        private readonly IConfiguration _configuration;
        private readonly ILogger<GenerateBarcodeService> _logger;
        private readonly HttpClient _httpClient;

        public GenerateBarcodeService(
            IBarcodeImplementationFactory barcodeFactory, 
            IConfiguration configuration,
            ILogger<GenerateBarcodeService> logger,
            HttpClient httpClient
            )
        {
            _barcodeFactory = barcodeFactory;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }
        
        public async Task GenerateBarcode(Guid bankId, Guid batchId, string barcodeServiceName, IEnumerable<CheckOrderBarcodeDto> checkOrders)
        {
            var barcodeImplementation = _barcodeFactory.GetBarcodeImplementation(barcodeServiceName);
            var checkOrderList = checkOrders.ToList();
            int total = checkOrderList.Count;
            var allUpdates = new List<UpdateCheckOrderBarcodeDto>();

            for (int i = 0; i < checkOrderList.Count; i++)
            {
                await NotifyBarcodeProgress(batchId, i + 1, total);
                var results = await barcodeImplementation.GenerateBarcode(bankId, batchId, new[] { checkOrderList[i] });
                allUpdates.AddRange(results);
            }

            await UpdateBarcodeValues(bankId, batchId, allUpdates);

            await GenerateReport(batchId);
        }

        private async Task NotifyBarcodeProgress(Guid batchId, int current, int total)
        {
            var baseUri = _configuration["Endpoints:CaptiveCommands"];
            if (string.IsNullOrEmpty(baseUri))
                return;

            try
            {
                var requestUri = $"{baseUri}/api/report/BatchProgress/{batchId}";
                var json = System.Text.Json.JsonSerializer.Serialize($"Generating barcodes ({current} of {total})");
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await _httpClient.PostAsync(requestUri, content);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to report barcode progress for batch {BatchId}", batchId);
            }
        }

        private async Task GenerateReport(Guid batchId)
        {

            var baseUri = _configuration["Endpoints:CaptiveCommands"];

            var requestUri = $"{baseUri}/api/report/GenerateOutput/{batchId}";

            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(requestUri, content);
        }


        private async Task UpdateBarcodeValues(Guid bankId, Guid batchId, IEnumerable<UpdateCheckOrderBarcodeDto> barcodeUpdates)
        {
            if (!barcodeUpdates.Any())
            {
                _logger.LogInformation("No barcode updates to process");
                return;
            }

            var baseUri = _configuration["Endpoints:CaptiveCommands"];
            if (string.IsNullOrEmpty(baseUri))
            {
                _logger.LogError("CaptiveCommands endpoint configuration is missing");
                throw new InvalidOperationException("CaptiveCommands endpoint configuration is missing");
            }

            var requestUri = $"{baseUri}/api/{bankId}/CheckOrder/UpdateCheckOrderBarCode";

            var requestBody = new
            {
                BatchId = batchId,
                CheckOrdersToUpdate = barcodeUpdates
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Updating {barcodeUpdates.Count()} barcode values via API: {requestUri}");

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully updated {barcodeUpdates.Count()} barcode values");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to update barcode values. Status: {response.StatusCode}, Error: {errorContent}");
                    throw new HttpRequestException($"Failed to update barcode values. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating barcode values");
                throw;
            }
        }
    }
}
