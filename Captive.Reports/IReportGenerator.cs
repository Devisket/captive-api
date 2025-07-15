using Captive.Data.Models;

namespace Captive.Reports
{
    public interface IReportGenerator
    {
        Task OnGenerateReport(Guid batchFileId, CancellationToken cancellationToken);
        Task GenerateBarcode(BankInfo bankInfo, Guid batchFileId, CancellationToken cancellationToken);
    }
}
