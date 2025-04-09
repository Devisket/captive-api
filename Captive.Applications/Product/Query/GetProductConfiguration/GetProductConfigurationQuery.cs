using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Product.Query.GetProductConfiguration
{
    public class GetProductConfigurationQuery:IRequest<ProductConfigurationDto>
    {
        public Guid ProductId { get; set; }
    }
}
