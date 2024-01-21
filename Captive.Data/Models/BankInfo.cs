
using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class BankInfo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BankName { get; set; }

        public string? BankDescription { get; set;}

        [Required]
        public DateTime CreatedDate { get; set; }

        public ICollection<BankBranches> BankBranches { get; set; }
    }
}
