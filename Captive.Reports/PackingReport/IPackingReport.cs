using Captive.Data.Models;
using Captive.Model.Dto.Reports;


namespace Captive.Reports.PackingReport
{
    public interface IPackingReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrderReport> checkDto, string filePath, CancellationToken cancellationToken);
    }
}
