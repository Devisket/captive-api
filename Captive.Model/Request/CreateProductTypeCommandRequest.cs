
namespace Captive.Applications.Product.Command.CreateProductType
{
    public class CreateProductTypeCommandRequest
    {
        public required string ProductName { get; set; }
        public required int ProductSequence { get; set; }
        public string? CustomizeFileName { get; set; }
    }
}
