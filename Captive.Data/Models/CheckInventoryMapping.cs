namespace Captive.Data.Models
{
    public class CheckInventoryMapping
    {
        public Guid Id { get; set; }
        public Guid CheckInventoryId { get; set; }
        public CheckInventory CheckInventory { get; set; } = null!;
        public Guid? BranchId { get; set; }
        public Guid? ProductId { get; set; }
        public string? FormCheckType { get; set; }
    }
}
