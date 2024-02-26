
namespace Captive.Applications.Product.Query.GetAllProductConfiguration.Model
{
    public  class ProductConfigurationResponse
    {
        public int Id { get; set; } 

        public int ProductTypeId { get; set; }
        public required string ProductTypeName { get; set; }

        public int ConfigurationId { get; set; }
        public required string ConfigurationName { get; set; }
    }
}
