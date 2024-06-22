
using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class BankInfo
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string BankName { get; set; }

        public string? BankDescription { get; set;}

        [Required]
        public required DateTime CreatedDate { get; set; }

        public required string ShortName { get; set; }

        public ICollection<BankBranches>? BankBranches { get; set; }
        public ICollection<FormChecks>? FormChecks { get; set; }
        public ICollection<OrderFileConfiguration>? OrderFileConfigurations { get; set;}
        public ICollection<BatchFile>? BatchFiles { get;set; }
        public ICollection<ProductType>? ProductTypes { get; set; }
    }
}
