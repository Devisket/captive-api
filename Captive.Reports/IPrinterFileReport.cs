using Captive.Processing.Processor.Model;

namespace Captive.Reports
{
    public interface IPrinterFileReport
    {
        public void GenerateReport(ICollection<OrderFileData> datas, string savePath);
    }
}
