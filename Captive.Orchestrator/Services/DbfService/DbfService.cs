using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Captive.Orchestrator.Services.DbfService
{
    public interface IDbfService 
    {
        Task GenerateDbfFile(Guid batchId);
    }
    public  class DbfService : IDbfService
    {
        private readonly IConfiguration _configuration;

        public DbfService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task GenerateDbfFile(Guid batchId)
        {
            var reqBody = new
            {
                batchId
            };

            var baseUri = string.Concat(_configuration["Endpoints:MdbApi"], "/api/processor/GenerateDbf");

            var client = new HttpClient();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(reqBody), System.Text.Encoding.UTF8, "application/json");

            await client.PostAsync(baseUri, content);
        }
    }
}
