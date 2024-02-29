using Captive.Data.Models;

namespace Captive.Reports
{
    public interface IReportGenerator
    {
        Task OnGenerateReport(int batchFileId, CancellationToken cancellationToken);
    }
}
