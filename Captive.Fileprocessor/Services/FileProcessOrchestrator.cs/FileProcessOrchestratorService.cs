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

        public async Task ProcessFile(FileUploadMessage message)
        {
            var files = message.Files;
            try
            {
                foreach (var file in files)
                {
                    var fileExtension = Path.GetExtension(file.FileName);

                    fileExtension = fileExtension.SanitizeFileName();

                    switch (fileExtension)
                    {
                        case ".txt":
                            break;
                        case ".xlsx":
                            break;
                        case ".mdb":
                            var orderFiles = await ExtractMdb(file);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
            }            
        }

        public async Task<IEnumerable<OrderFileData>> ExtractMdb(OrderfileDto orderFile)
        {
            IEnumerable<OrderFileData> orderfileDatas = new List<OrderFileData>();

            var baseUri = string.Concat(_configuration["Endpoints:MdbApi"], "/api/Mdb");

            var client = new HttpClient();

            var request = new HttpRequestMessage();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(orderFile), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUri, content);

            string responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {              
                if (string.IsNullOrEmpty(responseData))
                    throw new Exception("Empty response");

                orderfileDatas = JsonConvert.DeserializeObject<List<OrderFileData>>(responseData);

            } else {
                throw new Exception(responseData);
            }

            return orderfileDatas;
        }
    }
}
