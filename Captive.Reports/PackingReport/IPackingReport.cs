using Captive.Data.Models;


namespace Captive.Reports.PackingReport
{
    public interface IPackingReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken);
    }
}
