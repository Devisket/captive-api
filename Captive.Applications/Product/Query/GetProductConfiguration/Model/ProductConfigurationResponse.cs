
using Captive.Data.Enums;

namespace Captive.Applications.Product.Query.GetProductConfiguration.Model
{
    public  class ProductConfigurationResponse
    {
        public Guid Id { get; set; } 
        public Guid ProductId { get; set; }
        public required string FileName { get; set; }
        public required string ProductName { get; set; }
        public required string ConfigurationData { get; set; }
        public required ConfigurationType ConfigurationType { get; set; }
    }
}
