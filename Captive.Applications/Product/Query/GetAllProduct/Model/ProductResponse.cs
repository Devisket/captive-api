
namespace Captive.Applications.Product.Query.GetAllProduct.Model
{
    public  class ProductResponse
    {
        public Guid ProductTypeId { get; set; }
        public required string ProductTypeName { get; set; }
    }
}
