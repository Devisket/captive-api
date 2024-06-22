using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class OrderFileConfiguration
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ConfigurationData { get; set; }
        public string? OtherFileName { get; set; }
        public required ConfigurationType ConfigurationType { get; set; }
        public Guid BankId { get; set; }
        public BankInfo Bank { get; set; }
        public ICollection<ProductConfiguration>? ProductConfigurations { get; set; } 
    }
}
