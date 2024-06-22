
namespace Captive.Data.Models
{
    public class CheckInventory
    {
        public Guid Id { get; set; }
        public string? StarSeries { get; set; }
        public string? EndSeries { get; set; }

        public bool IsReserve { get;set; }
        public required int Quantity { get; set; }
        
        public Guid? CheckOrderId {get;set;}

        public Guid FormCheckId { get; set; }
        public  FormChecks FormChecks { get; set; }  

        public Guid BranchId { get; set; }
        public  BankBranches BankBranch;

        public string Tag { get; set; }
    }
}
