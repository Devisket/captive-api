
namespace Captive.Applications.Product.Query.GetAllProduct.Model
{
    public  class ProductResponse
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }

        public required int ProductSequence { get; set; }
    }
}
