
namespace Captive.Data.Models
{
    public class FloatingCheckOrder
    {
        public Guid Id { get; set; }
        public string? BranchCode { get; set; }
        public string AccountNo { get; set; }
        public required string BRSTN { get; set; }
        public required string AccountName { get; set; }
        public string? AccountName1 { get; set; }
        public string? AccountName2 { get; set; }
        public string? Concode { get; set; }
        public required int OrderQuanity { get; set; }
        public string? DeliverTo { get; set; }
        public string? PreStartingSeries { get; set; }
        public string? PreEndingSeries { get; set; }
        public int Quantity { get; set; }
        public Guid OrderFileId { get; set; }
    }
}