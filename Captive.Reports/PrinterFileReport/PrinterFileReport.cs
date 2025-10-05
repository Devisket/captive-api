using Captive.Data.Models;
using System.Text.RegularExpressions;

namespace Captive.Reports.PrinterFileReport
{
    public class PrinterFileReport : IPrinterFileReport
    {
        private readonly IReportService _reportService;
        public PrinterFileReport(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var branches = await _reportService.GetAlLBranches(batchFile.BankInfoId, cancellationToken);

            var checkDto = await _reportService.ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName, x.CustomizeFileName});

            foreach (var productCheckOrder in productGroup)
            {
                var productName = productCheckOrder.Key.ProductTypeName;

                var fileName = productCheckOrder.Key.CustomizeFileName ?? "PrinterFile";

                var productFilePath = Path.Combine(filePath, productCheckOrder.Key.ProductTypeName, $"{fileName}{productCheckOrder.Key.FormCheckName.First()}A.txt");

                using (StreamWriter writer = new StreamWriter(productFilePath, true))
                {
                    foreach (var checkOrder in productCheckOrder.OrderBy(x => x.BankBranch.BRSTNCode).ThenBy(x => x.CheckOrder.AccountNo).ThenBy(x => x.StartSeries))
                    {
                        RenderText(writer, checkOrder.CheckOrder, checkOrder.BankBranch, checkOrder.SeriesPattern, checkOrder.StartSeries, checkOrder.EndSeries, checkOrder.CheckType, checkOrder.BarcodeValue, checkOrder.NoOfPadding, checkOrder.AccountNumberFormat);
                    }
                }
            }
        }

        private void RenderText(StreamWriter writer, CheckOrders checkOrder, BankBranches branch, string seriesPattern, string startingSeries, string endingSeries, string CheckType, string? checkBarcodeValue, int noOfPadding, string? accountNumberFormat)
        {
            var concodes = string.IsNullOrEmpty(checkOrder.Concode) ? null : checkOrder.Concode.Split(";");

            var barcodeValues = !string.IsNullOrEmpty(checkBarcodeValue) ? checkBarcodeValue!.Split(';') : new string[] { };

            var nextStartSeries = GetNextStartingSeries(seriesPattern, endingSeries, noOfPadding);

            writer.WriteLine(5);
            writer.WriteLine(checkOrder.BRSTN);
            writer.WriteLine(checkOrder.AccountNo);
            writer.WriteLine(nextStartSeries);
            writer.WriteLine(CheckType);
            writer.WriteLine();
            writer.WriteLine(checkOrder.BRSTN.Substring(0, 5));
            writer.WriteLine(string.Format(" {0}", checkOrder.BRSTN.Substring(5, 4)));

            var accNo = checkOrder.AccountNo;

            if (!string.IsNullOrEmpty(accountNumberFormat))
                accNo = Regex.Replace(checkOrder.AccountNo, $"{accountNumberFormat}", @"$1-$2-$3");

            writer.WriteLine(accNo);
            writer.WriteLine(concodes == null ? checkOrder.AccountName : concodes[0]);
            writer.WriteLine("SN");
            writer.WriteLine(string.Empty);
            writer.WriteLine(concodes != null && concodes.Count() > 1 ? concodes[1] : string.Empty);
            writer.WriteLine("C");
            writer.WriteLine("XXXX");
            writer.WriteLine(concodes != null && concodes.Count() > 2 ? concodes[2] : string.Empty);
            writer.WriteLine(branch.BranchName);
            writer.WriteLine(branch.BranchAddress1);
            writer.WriteLine(branch.BranchAddress2);
            writer.WriteLine(branch.BranchAddress3);
            writer.WriteLine(branch.BranchAddress4);
            writer.WriteLine(branch.BranchAddress5);
            writer.WriteLine(branch.BankInfo.BankName);
            writer.WriteLine("\n \n \n \n \n \n");

            writer.WriteLine(startingSeries);
            writer.WriteLine(endingSeries);

            foreach (var barcodeValue in barcodeValues)
            {
                writer.WriteLine($"*{barcodeValue}*");
            }

            foreach (var barcodeValue in barcodeValues)
            {
                writer.WriteLine($"{barcodeValue}");
            }
        }


        private string GetNextStartingSeries(string pattern, string endingSeries, int noOfPadding)
        {
            var numerical = new long();
            var numericalString = endingSeries;

            if (!string.IsNullOrEmpty(pattern))
            {
                numericalString = endingSeries.Replace(pattern, string.Empty);
            }
            
            numerical = long.Parse(numericalString);
            numerical += 1;


            if(!string.IsNullOrEmpty(pattern))
            {
                return string.Concat(pattern, numericalString.PadLeft(noOfPadding, '0'));
            }

            return numerical.ToString().PadLeft(noOfPadding,'0');
        }
    }
}
