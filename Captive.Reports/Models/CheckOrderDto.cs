
using Captive.Data.Models;

namespace Captive.Reports.Models
{
    public class CheckOrderDTO
    {
        public required string ProductTypeName { get; set; }
        public string? FormCheckName { get; set; }
        public required CheckOrders CheckOrder { get; set; }
        public int CheckInventoryId { get; set; }
        public string StartSeries { get; set; }
        public string EndSeries { get; set; }
        public required BankBranches BankBranch { get; set; }
    }
}
