using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class CheckOrders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AccountNo { get; set; }

        [Required]
        public int OrderFileId { get; set; }
        public OrderFiles OrderFile { get; set; }

        [Required]
        public string BRSTN { get; set; }


        public string? DeliverTo { get; set; }

        [Required]
        public int CheckAccountId { get; set; }
        public CheckAccounts CheckAccount { get; set; }
    }
}
