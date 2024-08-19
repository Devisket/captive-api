using Captive.Data.Enums;
namespace Captive.Data.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string TagName { get; set;}
        public TagType TagType { get; set;}
        public Guid BankInfoId { get; set;}
        public BankInfo BankInfo { get; set; }

        public ICollection<CheckInventory> CheckInventory { get; set;}
        public ICollection<Product> Products { get; set; }
        public ICollection<FormChecks> FormChecks { get; set; }
        public ICollection<BankBranches> BankBranches { get; set; }
        public ICollection<TagMapping> TagMappings { get; set; }
    }
}
