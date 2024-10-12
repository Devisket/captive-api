
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }
        public Guid CheckValidationId { get; set; }
        public CheckValidation CheckValidation { get; set; }
        public ICollection<CheckInventoryDetail> CheckInventoryDetails { get; set; }
    }
}
