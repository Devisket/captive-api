using Captive.Data.Models;

namespace Captive.Reports
{
    public interface IReportGenerator
    {
        Task OnGenerateReport(Guid batchFileId, CancellationToken cancellationToken);
    }
}
