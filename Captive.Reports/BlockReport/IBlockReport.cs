using Captive.Data.Models;

namespace Captive.Reports.BlockReport
{
    public interface IBlockReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken);
    }
}
