using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class AccountInfo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CheckAccountId { get; set; }
        public CheckAccounts CheckAccount{ get; set; }

        [Required]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }

        public int? AccountAddressId { get; set; }
        public AccountAddresses? AccountAddress { get; set; }
    }
}
