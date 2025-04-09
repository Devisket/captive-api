using Captive.Applications.Product.Query.GetAllProduct.Model;
using MediatR;

namespace Captive.Applications.Product.Query.GetAllProduct
{
    public class GetAllProductTypeQuery :IRequest<ICollection<ProductResponse>>
    {
        public Guid BankId { get; set; }
    }
}
