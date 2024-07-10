using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class CheckOrders
    {
        public Guid Id { get; set; }
        public string AccountNo { get; set; }
        public Guid OrderFileId { get; set; }
        public OrderFile OrderFile { get; set; }
        public Guid FormCheckId { get; set; }
        public FormChecks FormChecks { get; set; }
        public required string BRSTN { get; set; }
        public required string AccountName { get; set; }
        public string? Concode { get; set; }
        public required int OrderQuanity { get; set; }
        public string? DeliverTo { get; set; }
        public bool InputEnable { get;set; }
        public string ErrorMessage { get; set; }
    }
}
