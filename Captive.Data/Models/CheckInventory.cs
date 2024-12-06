
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }        
        /**
         * SERIES PATTERN: 
         * (A-Z) - any letter
         * 0 - incremental numerical value 
         * example: "ABC0000" 
         * system will create out of this pattern as "ABC0001", "ABC0002"
         * but if the check order has pre-defined series, the system will disregard this pattern.
         */
        public string SeriesPatern { get; set; }
        public Guid CheckValidationId { get; set; }
        public CheckValidation CheckValidation { get; set; }
        public ICollection<CheckInventoryDetail> CheckInventoryDetails { get; set; }
    }
}
