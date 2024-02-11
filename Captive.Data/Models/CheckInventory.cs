
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public CheckInventory() { }

        public int Id { get; set; }
        public string? StarSeries { get; set; }
        public string? EndSeries { get; set; }
        public required int Quantity { get; set; }
        
        public int? CheckOrderId {get;set;}

        public int FormCheckId { get; set; }
        public FormChecks FormChecks { get; set; }  
    }
}
