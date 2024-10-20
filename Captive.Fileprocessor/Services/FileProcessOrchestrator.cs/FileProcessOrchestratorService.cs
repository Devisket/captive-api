using Captive.Data.Models;
using Captive.Messaging.Models;
using Captive.Model;
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
         * 1. Create a batch
         * 2. Send into file processor
         * 3. Extract the orderfile
         * 4. Validate every orderfile
         *      a. If there is invalid record update the order file error message and status
         * 4. Save into the database
         * 5. Update the order file status
         * 6. Update the batch file status overall
         */

        public async Task ProcessFile(FileUploadMessage message)
        {
            var files = message.Files;

            try
            {
                foreach (var file in files)
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
                        continue;
                    }
                    
                    await ValidateCheckOrder(checkOrders, file.Id, message.BankId, file.FileName);
                }
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
            }
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

                if(orderfileDatas == null)
                    throw new Exception($"Failed to map check orders for  {orderFile.FileName}");

                return orderfileDatas.ToList();
            } else {
                throw new Exception(responseData);
            }            
        }


        public async Task<(bool,List<CheckOrderDto>)> ValidateCheckOrder(List<CheckOrderDto> checkOrder, Guid OrderId, Guid bankId, string fileName )
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

                if (validateCheckDto == null ) 
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
    }
}
