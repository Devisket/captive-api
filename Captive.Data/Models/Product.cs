
namespace Captive.Data.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }

        public Guid BankInfoId { get; set; }
        public BankInfo? BankInfo { get; set; }

        public ICollection<FormChecks>? FormChecks { get; set; }
    }
}
