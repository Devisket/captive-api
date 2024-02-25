using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class OrderFileConfiguration
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ConfigurationData { get; set; }
        public required ConfigurationType ConfigurationType { get; set; }
        public int BankId { get; set; }
        public BankInfo Bank { get; set; }
        public ICollection<ProductConfiguration>? ProductConfigurations { get; set; } 
    }
}
