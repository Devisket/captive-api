
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }

        public Guid? TagId { get; set; }
        public Tag? Tag;

        public ICollection<CheckInventoryDetail> CheckInventoryDetails { get; set; }
    }
}
