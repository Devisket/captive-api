
using MediatR;

namespace Captive.Applications.Product.Command.CreateProductType
{
    public class CreateProductTypeCommand : IRequest<Unit>
    {
        public Guid BankId { get; set; }
        public Guid? ProductId { get; set; }
        public required string ProductName { get; set; }
    }
}
