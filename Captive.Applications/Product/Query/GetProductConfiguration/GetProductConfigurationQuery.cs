
using Captive.Applications.Product.Query.GetProductConfiguration.Model;
using MediatR;

namespace Captive.Applications.Product.Query.GetProductConfiguration
{
    public class GetProductConfigurationQuery:IRequest<GetAllProductConfigurationQueryResponse>
    {
        public Guid ProductId { get; set; }
    }
    public class GetAllProductConfigurationQueryResponse
    {
        public ProductConfigurationResponse ProductConfiguration { get; set; }
    }
}
