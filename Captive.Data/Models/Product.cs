
namespace Captive.Data.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }

        public Guid BankInfoId { get; set; }
        public BankInfo? BankInfo { get; set; }

        public Guid? TagId { get; set; }
        public Tag? Tag { get; set; }

        public ICollection<FormChecks>? FormChecks { get; set; }
        public ICollection<ProductConfiguration>? ProductConfiguration { get; set; }
    }
}
