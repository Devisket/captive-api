using Captive.Processing.Processor.Model;

namespace Captive.Reports
{
    public class PrinterFileReport : IPrinterFileReport
    {
        public void GenerateReport(ICollection<OrderFileData> datas, string savePath)
        {
            using (var writer  = new StringWriter())
            {


            }
         
        }
    }
}
