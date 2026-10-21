using Captive.Data.Models;
using Captive.Model.Dto.Reports;

namespace Captive.Reports.PrinterFileReport
{
    public interface IPrinterFileReport
    {
        Task GenerateReport(BatchFile batchFile, ICollection<CheckOrderReport> checkDto, string filePath, CancellationToken cancellationToken);
    }
}
