
using MediatR;

namespace Captive.Applications.Product.Command.CreateProductType
{
    public class CreateProductTypeCommand : IRequest<Unit>
    {
        public int BankId { get; set; }

        public int ProductTypeId { get; set; }
        public required string ProductName { get; set; }
    }
}
