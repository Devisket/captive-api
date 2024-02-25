
namespace Captive.Data.Models
{
    public  class ProductConfiguration
    {
        public int Id { get; set; }

        public required int OrderFileConfigurationId { get; set; }
        public required OrderFileConfiguration OrderFileConfiguration { get; set; }

        public required int ProductTypeId { get; set; }  
        public required ProductType ProductType { get; set; }
    }
}
