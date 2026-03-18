
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }    
        public required string SeriesPatern { get; set; }
        public long StartingSeries { get; set; }
        public long WarningSeries {  get; set; }
        public long EndingSeries { get; set; }
        public long CurrentSeries { get; set; }
        public int NumberOfPadding {  get; set; }
        public bool isRepeating {  get; set; }
        public bool IsActive { get; set; }
        public bool IsDeprecated { get; set; }
        public Guid BankId { get; set; }
        public string? AccountNumber { get; set; } = string.Empty;
        public ICollection<CheckInventoryDetail>? CheckInventoryDetails { get; set; }
        public ICollection<CheckInventoryMapping> Mappings { get; set; } = new List<CheckInventoryMapping>();
    }
}
