using Captive.Data.Models;
using Captive.Processing.Processor.Model;

namespace Captive.Reports
{
    public interface IPrinterFileReport
    {
        Task GenerateReport(OrderFile orderFile, BankInfo bank, CancellationToken cancellationToken);
    }
}
