using Captive.Data.Models;
using Captive.Model.Dto.Reports;

namespace Captive.Reports.BlockReport
{
    public interface IBlockReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrderReport> checkDto, string filePath, CancellationToken cancellationToken);
    }
}
