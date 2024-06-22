using MediatR;

namespace Captive.Applications.Product.Command.DeleteProductType
{
    public class DeleteProductTypeCommand : IRequest<Unit> { 
    
        public Guid BankId { get; set; }

        public Guid ProductTypeId { get; set; }
    }
}
