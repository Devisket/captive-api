
namespace Captive.Data.Models
{
    public  class ProductConfiguration
    {
        public Guid Id { get; set; }

        public required Guid OrderFileConfigurationId { get; set; }
        public required OrderFileConfiguration OrderFileConfiguration { get; set; }

        public required Guid ProductTypeId { get; set; }  
        public required ProductType ProductType { get; set; }
    }
}
