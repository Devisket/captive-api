using Captive.Applications.Product.Query.GetAllProduct.Model;
using MediatR;

namespace Captive.Applications.Product.Query.GetAllProduct
{
    public class GetAllProductTypeQuery :IRequest<GetAllProductTypeQueryResponse>
    {
        public Guid BankId { get; set; }
    }
    public class GetAllProductTypeQueryResponse
    {
        public Guid BankId { get; set; }
        public ICollection<ProductResponse>? ProductTypes { get; set; }
    }
}
