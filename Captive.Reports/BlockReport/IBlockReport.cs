using Captive.Data.Models;
using Captive.Reports.Models;

namespace Captive.Reports.BlockReport
{
    public interface IBlockReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken);
    }
}
