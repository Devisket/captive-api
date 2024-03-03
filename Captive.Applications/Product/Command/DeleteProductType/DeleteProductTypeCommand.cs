using MediatR;

namespace Captive.Applications.Product.Command.DeleteProductType
{
    public class DeleteProductTypeCommand : IRequest<Unit> { 
    
        public int BankId { get; set; }

        public int ProductTypeId { get; set; }
    }
}
