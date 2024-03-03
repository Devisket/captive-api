using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class BankBranches
    {
        [Key]
        public int Id { get; set; }

        public int BankId { get; set; }
        public BankInfo BankInfo { get; set; }

        [Required]
        public required string BranchName {  get; set; }

        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
        public string? BranchAddress4 { get; set; }
        public string? BranchAddress5 { get; set; }

        [Required]
        public required string BRSTNCode {  get; set; }

        public ICollection<CheckInventory>? CheckInventory { get; set; }
    }
}
