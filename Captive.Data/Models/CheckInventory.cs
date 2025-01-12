
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }    
        public string SeriesPatern { get; set; }
        public int StartingSeries { get; set; }
        public int WarningSeries {  get; set; }
        public int NumberOfPadding {  get; set; }
        public bool isRepeating {  get; set; }
        public Guid CheckValidationId { get; set; }
        public CheckValidation CheckValidation { get; set; }
        public ICollection<CheckInventoryDetail> CheckInventoryDetails { get; set; }
    }
}
