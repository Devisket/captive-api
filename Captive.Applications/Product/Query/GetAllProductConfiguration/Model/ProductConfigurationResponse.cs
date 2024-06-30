
namespace Captive.Applications.Product.Query.GetAllProductConfiguration.Model
{
    public  class ProductConfigurationResponse
    {
        public Guid Id { get; set; } 

        public Guid ProductTypeId { get; set; }
        public required string ProductTypeName { get; set; }
        public required string ConfigurationData { get; set; }
    }
}
