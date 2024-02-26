
using Captive.Applications.Product.Query.GetAllProductConfiguration.Model;
using MediatR;

namespace Captive.Applications.Product.Query.GetAllProductConfiguration
{
    public class GetAllProductConfigurationQuery:IRequest<GetAllProductConfigurationQueryResponse>
    {
        public int BankId { get; set; }
    }
    public class GetAllProductConfigurationQueryResponse
    {
        public GetAllProductConfigurationQueryResponse(int bankId) { 
            this.BankId = bankId; 
            ProductConfigurations = new List<ProductConfigurationResponse>();  
        }

        public int BankId { get; set; }
        public ICollection<ProductConfigurationResponse> ProductConfigurations { get; set; }
    }
}
