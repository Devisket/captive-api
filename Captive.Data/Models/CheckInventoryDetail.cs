
namespace Captive.Data.Models
{
    public class CheckInventoryDetail
    {
        public Guid Id { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? FormCheckId { get; set; }
        public Guid? TagId { get; set; } 

        public string? StartingSeries { get; set; }
        public string? EndingSeries { get; set; }
        public string? AccountNumber {  get; set; }

        public bool IsReserve { get; set; }
        public required int Quantity { get; set; }
        public Guid? CheckOrderId { get; set; }
        public CheckOrders? CheckOrder{ get; set; }

        public Guid CheckInventoryId { get; set; }
        public CheckInventory? CheckInventory {  get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
