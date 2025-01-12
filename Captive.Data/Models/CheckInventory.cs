
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
        public Guid TagId { get; set; } 
        public Tag Tag { get; set; }    
        public ICollection<CheckInventoryDetail> CheckInventoryDetails { get; set; }
    }
}
