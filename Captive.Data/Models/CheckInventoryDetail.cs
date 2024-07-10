
namespace Captive.Data.Models
{
    public class CheckInventoryDetail
    {
        public Guid Id { get; set; }

        public string? StarSeries { get; set; }
        public string? EndSeries { get; set; }

        public bool IsReserve { get; set; }
        public required int Quantity { get; set; }

        public Guid? CheckOrderId { get; set; }
        public CheckOrders? CheckOrder{ get; set; }

        public Guid CheckInventoryId { get; set; }
        public CheckInventory CheckInventory {  get; set; }
    }
}
