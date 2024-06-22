using Captive.Applications.Product.Query.GetAllProductType.Model;
using MediatR;

namespace Captive.Applications.Product.Query.GetAllProductType
{
    public class GetAllProductTypeQuery :IRequest<GetAllProductTypeQueryResponse>
    {
        public Guid BankId { get; set; }
    }
    public class GetAllProductTypeQueryResponse
    {
        public Guid BankId { get; set; }
        public ICollection<ProductTypeResponse>? ProductTypes { get; set; }
    }
}
