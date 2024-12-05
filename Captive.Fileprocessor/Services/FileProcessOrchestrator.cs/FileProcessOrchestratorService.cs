using Captive.Data.Enums;
using Captive.Messaging.Models;
using Captive.Model.Dto;
using Captive.Utility;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Captive.Fileprocessor.Services.FileProcessOrchestrator.cs
{
    class FileProcessOrchestratorService : IFileProcessOrchestratorService
    {
        private readonly IConfiguration _configuration;
        private string baseUri;
        public FileProcessOrchestratorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /*
         * Need tow work
         * 1. Create a batch [X]
         * 2. Send into file processor [X]
         * 3. Extract the orderfile [X]
         * 4. Validate every orderfile [X]
         *      a. If there is invalid record update the order file error message and status
         * 4. Save into the database
         * 5. Update the order file status
         * 6. Update the batch file status overall
         */

        public async Task ProcessFile(FileUploadMessage message)
        {
            var files = message.Files;
            foreach (var file in files)
            {
                try
                {
                    List<CheckOrderDto> checkOrders = new List<CheckOrderDto>();

                    var fileExtension = Path.GetExtension(file.FileName);

                    fileExtension = fileExtension.SanitizeFileName();

                    switch (fileExtension)
                    {
                        case ".txt":
                            break;
                        case ".xlsx":
                            break;
                        case ".mdb":
                            checkOrders = await ExtractMdb(file);
                            break;
                        default:
                            break;
                    }

                    if (checkOrders == null || !checkOrders.Any())
                    {
                        await SendOrderFileStatus(file.Id, "Can't map the check orders for this file", OrderFilesStatus.Error);
                        continue;
                    }

                    (var isValid, var validatedCheckOrders) = await ValidateCheckOrder(checkOrders, file.Id, message.BankId, file.FileName);

                    if (!isValid)
                    {
                        await SendOrderFileStatus(file.Id, "Error in one of the check order", OrderFilesStatus.Error);
                    }
                    else
                    {
                        await SendOrderFileStatus(file.Id, string.Empty, OrderFilesStatus.Completed);
                    }

                    await CreateCheckOrder(file.Id, message.BankId,  validatedCheckOrders);
                    await ApplyCheckInventoryDetails(file.Id);
                }
                catch (Exception ex)
                {
                    await SendOrderFileStatus(file.Id, ex.Message, OrderFilesStatus.Error);
                    continue;
                }
            }

            await GenerateReport(message.BatchID);
        }

        private async Task<List<CheckOrderDto>> ExtractMdb(OrderfileDto orderFile)
        {
            IEnumerable<CheckOrderDto> orderfileDatas = new List<CheckOrderDto>();

            var baseUri = string.Concat(_configuration["Endpoints:MdbApi"], "/api/Mdb");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(orderFile), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseData))
                    throw new Exception($"Failed to get data from MDB processor for {orderFile.FileName}");

                orderfileDatas = JsonConvert.DeserializeObject<List<CheckOrderDto>>(responseData);

                if (orderfileDatas == null)
                    throw new Exception($"Failed to map check orders for  {orderFile.FileName}");

                return orderfileDatas.ToList();
            }
            else
            {
                throw new Exception(responseData);
            }
        }

        private async Task<(bool, List<CheckOrderDto>)> ValidateCheckOrder(List<CheckOrderDto> checkOrder, Guid OrderId, Guid bankId, string fileName)
        {
            var reqBody = new
            {
                OrderId,
                checkOrder,
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/{bankId}/checkOrder/validateCheck");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseData))
                    throw new Exception($"Failed to validate {fileName}");

                var validateCheckDto = JsonConvert.DeserializeObject<ValidateCheckOrderDto>(responseData);

                if (validateCheckDto == null)
                {
                    throw new Exception($"Cannot deserialize response from Check Order Validation API ");
                }

                return (validateCheckDto.IsValid, validateCheckDto.CheckOrder.ToList());
            }
            else
            {
                throw new Exception(responseData);
            }
        }

        private async Task SendOrderFileStatus(Guid orderFileId, string ErrorMessage, OrderFilesStatus status)
        {
            var reqBody = new
            {
                ErrorMessage,
                Status = status.ToString(),
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/orderFile/{orderFileId}/updateStatus");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update the status of OrderFileID  {orderFileId}");
            }
        }

        private async Task SendBatchFileStatus(Guid batchId, string ErrorMessage, OrderFilesStatus status)
        {
            var reqBody = new
            {
                ErrorMessage,
                Status = status.ToString(),
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/orderFile/{batchId}/updateStatus");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update the status of BatchID  {batchId}");
            }
        }

        private async Task CreateCheckOrder(Guid orderFileId, Guid bankId,  List<CheckOrderDto> checkOrders)
        {
            var reqBody = new
            {
                orderFileId,
                checkOrders
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/{bankId}/checkOrder");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create check order for order file ID: {orderFileId}");
            }
        }

        private async Task ApplyCheckInventoryDetails(Guid orderFileId)
        {
            var reqBody = new
            {
                orderFileId
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/checkInventory/ApplyCheckInventoryDetails");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update the status of OrderFileID  {orderFileId}");
            }
        }

        private async Task GenerateReport(Guid batchId)
        {
            var reqBody = new
            {
                batchId
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/report");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Cannot generate report for BatchID: {batchId}");
            }
        }


        private async Task ExtractTextFile(OrderfileDto orderFile) { }
        private async Task ExtractCsv(OrderfileDto orderFile) { }
        public async Task GenerateOrderFIle(Guid orderFileId)
        {
           
        }
    }
}
