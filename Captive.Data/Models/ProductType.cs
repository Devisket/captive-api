
namespace Captive.Data.Models
{
    public  class ProductType
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }

        public int BankInfoId { get; set; }
        public BankInfo? BankInfo { get; set; }

        public ICollection<FormChecks>? FormChecks { get; set; }
        public ICollection<ProductConfiguration>? ProductConfiguration { get; set; }
    }
}
