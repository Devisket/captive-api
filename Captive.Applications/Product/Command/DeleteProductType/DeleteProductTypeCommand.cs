using MediatR;

namespace Captive.Applications.Product.Command.DeleteProductType
{
    public class DeleteProductTypeCommand : IRequest<Unit> 
    { 
        public Guid ProductId { get; set; }
    }
}
