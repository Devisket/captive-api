
namespace Captive.Data.Models
{
    public class FloatingCheckOrder
    {
        public Guid Id { get; set; }
        public string? BranchCode { get; set; }
        public string AccountNo { get; set; } = string.Empty;
        public required string BRSTN { get; set; }
        public required string AccountName { get; set; }
        public string? AccountName1 { get; set; }
        public string? AccountName2 { get; set; }
        public string? Concode { get; set; }
        public string? DeliverTo { get; set; }
        public string? PreStartingSeries { get; set; }
        public string? PreEndingSeries { get; set; }
        public string FormType { get; set; } = string.Empty;
        public string CheckType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsValid {  get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsOnHold { get; set; } = false;
        public string? OrderNo { get; set; }
        public Guid OrderFileId { get; set; }
        public OrderFile OrderFile { get; set; }
    }
}