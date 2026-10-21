using Captive.Data.Models;
using Captive.Model.Dto.Reports;

namespace Captive.Reports.HashTotalReport
{
    public interface IHashTotalReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrderReport> checkDto, string filePath, CancellationToken cancellationToken);
    }
}
