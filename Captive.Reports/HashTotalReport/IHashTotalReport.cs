using Captive.Data.Models;

namespace Captive.Reports.HashTotalReport
{
    public interface IHashTotalReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken);
    }
}
