using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class OrderFileConfiguration
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ConfigurationData { get; set; }
        public string? FileName { get; set; }
        public required ConfigurationType ConfigurationType { get; set; }
        public Guid BankInfoId { get; set; }
        public BankInfo Bank { get; set; }
        public ICollection<ProductConfiguration>? ProductConfigurations { get; set; } 
    }
}
