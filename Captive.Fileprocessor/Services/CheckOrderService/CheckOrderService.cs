using Captive.Data.Enums;
using Captive.Model.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Captive.Fileprocessor.Services.CheckOrderService
{
    public interface ICheckOrderService
    {
        Task<List<CheckOrderDto>> ExtractMdb(OrderfileDto orderFile);
        Task<(bool, List<CheckOrderDto>)> ValidateCheckOrder(List<CheckOrderDto> checkOrder, Guid OrderId, Guid bankId, string fileName);
        Task SendOrderFileStatus(Guid orderFileId, string ErrorMessage, OrderFilesStatus status);
        Task SendOrderFileStatus(Guid orderFileId, OrderFilesStatus status);
        Task SendBatchFileStatus(Guid batchId, string ErrorMessage, OrderFilesStatus status);
        Task CreateFloatingCheckOrder(Guid orderFileId, Guid bankId, List<CheckOrderDto> checkOrders);
        Task ApplyCheckInventoryDetails(Guid orderFileId);
        Task GenerateReport(Guid batchId);
        Task ExtractTextFile(OrderfileDto orderFile);
        Task ExtractCsv(OrderfileDto orderFile);
        Task GenerateOrderFIle(Guid orderFileId);
    }
    public class CheckOrderService : ICheckOrderService
    {
        private readonly IConfiguration _configuration;
        public CheckOrderService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<List<CheckOrderDto>> ExtractMdb(OrderfileDto orderFile)
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

        public async Task<(bool, List<CheckOrderDto>)> ValidateCheckOrder(List<CheckOrderDto> checkOrder, Guid OrderId, Guid bankId, string fileName)
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

        public async Task SendOrderFileStatus(Guid orderFileId, OrderFilesStatus status)
        {
            var reqBody = new
            {
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

        public async Task SendOrderFileStatus(Guid orderFileId, string ErrorMessage, OrderFilesStatus status)
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

        public async Task SendBatchFileStatus(Guid batchId, string ErrorMessage, OrderFilesStatus status)
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

        public async Task CreateFloatingCheckOrder(Guid orderFileId, Guid bankId, List<CheckOrderDto> checkOrders)
        {
            var reqBody = new
            {
                orderFileId,
                checkOrders
            };

            var baseUri = string.Concat(_configuration["Endpoints:CaptiveCommands"], $"/api/{bankId}/checkOrder/floating");

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

        public async Task ApplyCheckInventoryDetails(Guid orderFileId)
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

        public async Task GenerateReport(Guid batchId)
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

        public async Task ExtractTextFile(OrderfileDto orderFile) { }

        public async Task ExtractCsv(OrderfileDto orderFile) { }

        public async Task GenerateOrderFIle(Guid orderFileId)
        {

        }
    }
}
