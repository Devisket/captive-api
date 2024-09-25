
using Captive.Applications.Product.Query.GetAllProductConfiguration.Model;
using MediatR;

namespace Captive.Applications.Product.Query.GetAllProductConfiguration
{
    public class GetAllProductConfigurationQuery:IRequest<GetAllProductConfigurationQueryResponse>
    {
        public Guid ProductId { get; set; }
    }
    public class GetAllProductConfigurationQueryResponse
    {
        public GetAllProductConfigurationQueryResponse() { 
            ProductConfigurations = new List<ProductConfigurationResponse>();  
        }
        public ICollection<ProductConfigurationResponse> ProductConfigurations { get; set; }
    }
}
