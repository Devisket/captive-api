
namespace Captive.Data.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }

        public Guid BankInfoId { get; set; }
        public BankInfo? BankInfo { get; set; }

        public ICollection<FormChecks>? FormChecks { get; set; }
        public ICollection<OrderFile>? OrderFiles { get; set; }

        public Guid ProductConfigurationId { get; set; }
        public ProductConfiguration ProductConfiguration { get; set; }
    }
}
