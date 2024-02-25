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
        public OrderFile OrderFile { get; set; }


        [Required]
        public int FormCheckId { get; set; }
        public FormChecks FormChecks { get; set; }

        [Required]
        public required string BRSTN { get; set; }

        [Required]
        public required string AccountName { get; set; }

        public required int OrderQuanity { get; set; }
        public string? DeliverTo { get; set; }
    }
}
