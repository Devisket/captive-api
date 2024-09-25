
namespace Captive.Applications.Product.Query.GetAllProductConfiguration.Model
{
    public  class ProductConfigurationResponse
    {
        public Guid Id { get; set; } 

        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ConfigurationData { get; set; }
    }
}
