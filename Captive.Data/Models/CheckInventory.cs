
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }    
        public string SeriesPatern { get; set; }
        public int LastSeriesNumber {  get; set; }
        public Guid CheckValidationId { get; set; }
        public CheckValidation CheckValidation { get; set; }
        public ICollection<CheckInventoryDetail> CheckInventoryDetails { get; set; }
    }
}
