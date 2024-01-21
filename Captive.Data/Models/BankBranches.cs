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
        public string BranchName {  get; set; }
        public string? BranchAddress { get; set; }

        [Required]
        public string BRSTNCode {  get; set; }
    }
}
