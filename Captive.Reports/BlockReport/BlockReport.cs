using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using System.Text.RegularExpressions;

namespace Captive.Reports.BlockReport
{
    public  class BlockReport : IBlockReport
    {
        private IReportService _reportService;

        public BlockReport(IReadUnitOfWork readUow, IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var branches = await _reportService.GetAlLBranches(batchFile.BankInfoId, cancellationToken);

            var checkDto = await _reportService.ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName });

            var formCheckTypeCount = checkDto.GroupBy(x => x.FormCheckType);

            int runningNo = 0, blockNo = 0, pageNo = 0;


            foreach (var productCheckOrder in productGroup)
            {
                var productName = productCheckOrder.Key.ProductTypeName;
                var formCheckName = productCheckOrder.Key.FormCheckName ?? string.Empty;
                var bankShortName = branches.First().BankInfo.ShortName;
                var fileName = productCheckOrder.First().OrderFileName;


                List<Tuple<string, int>> formcheckList = new List<Tuple<string, int>>();

                foreach (var checkType in formCheckTypeCount)
                {
                    var checkTypeInitial = checkType.Key == FormCheckType.Personal ? "A" : "B";

                    formcheckList.Add(new Tuple<string, int>(checkTypeInitial, checkType.Count()));
                }

                var productFilePath = Path.Combine(filePath, productCheckOrder.Key.ProductTypeName, $"Block{formCheckName?.First()}.txt");

                using (StreamWriter writer = new StreamWriter(productFilePath, true))
                {
                    foreach (var checkOrder in productCheckOrder.OrderBy(x => x.BankBranch.BRSTNCode).ThenBy(x => x.CheckOrder.AccountNo).ThenBy(x => x.StartSeries))
                    {
                        if ((blockNo % 8) == 0 && (runningNo % 4) == 0)
                        {
                            pageNo++;
                            RenderHeader(writer, bankShortName, productName, formCheckName, pageNo);
                        }

                        if ((runningNo % 4) == 0 || blockNo == 0)
                        {
                            blockNo++;
                            writer.WriteLine();
                            writer.WriteLine($" ** BLOCK{blockNo}");
                        }

                        RenderText(writer, checkOrder.CheckOrder, checkOrder.BankBranch, checkOrder.StartSeries, checkOrder.EndSeries, blockNo, checkOrder.AccountNumberFormat);

                        runningNo++;

                        if ((blockNo % 8) == 0 && (runningNo % 4) == 0 && pageNo == 1)
                            RenderFooter(writer, formcheckList, fileName, batchFile.DeliveryDate);
                    }
                    Console.WriteLine();

                    if (blockNo <= 4)
                        RenderFooter(writer, formcheckList,fileName, batchFile.DeliveryDate);

                    runningNo = 0;
                    blockNo = 0;
                    pageNo = 0;
                }
            }
        }

        private void RenderText(StreamWriter writer, CheckOrders checkOrder, BankBranches branch, string? startingSeries, string? endingSeries, int blockNo, string? accountNumberFormat)
        {
            var accNo = checkOrder.AccountNo;

            if(!string.IsNullOrEmpty(accountNumberFormat))
                accNo = Regex.Replace(checkOrder.AccountNo, $"{accountNumberFormat}", @"$1-$2-$3");

            writer.WriteLine($"\t  {blockNo} {branch.BRSTNCode}\t{accNo}\t\t{startingSeries.PadLeft(10, '0')} \t{endingSeries.PadLeft(10,'0')}");
        }

        private void RenderHeader(StreamWriter writer, string bankShortName, string productDescription, string formCheckDescription, int page)
        {
            writer.WriteLine();
            writer.WriteLine($"\t \t Page No.{page}");
            writer.WriteLine($"\t \t {DateTime.Now.ToString("dd/MM/yyyy")}");
            writer.WriteLine($"\t \t \t \t  {bankShortName.ToUpper()} - SUMMARY OF BLOCK - {formCheckDescription.ToUpper()} Check");
            writer.WriteLine($"\t \t \t \t \t \t \t \t    {productDescription.ToUpper()} Check");
            writer.Write("\n \n");
            writer.WriteLine("  BLOCK RT_NO\t\tACCT_NO\t\t\tSTART_NO.\tEND_NO.\t\tDELIVER_TO");
            writer.Write("\n \n");
            writer.WriteLine();
        }

        private void RenderFooter(StreamWriter writer, List<Tuple<string, int>> formcheckType, string fileName, DateTime deliveryDate)
        {
            writer.WriteLine();
            foreach (var item in formcheckType)
            {
                writer.Write($"\t {item.Item1}: {item.Item2}");

                if (item.Equals(formcheckType.First()))
                {
                    writer.Write($"\t\t\t\t\t\t {fileName}");
                    writer.Write($"\t\t\t\t\t\t DLV: {deliveryDate.ToString("MMM-dd")} ({deliveryDate.ToString("ddd")})");
                }

                writer.Write('\n');
            }

            writer.WriteLine($"\t Prepared By:");
            writer.WriteLine($"\t Updated By:");
            writer.WriteLine($"\t Time Start: {DateTime.Now.TimeOfDay}");
            writer.WriteLine($"\t Time Finished:{DateTime.Now.AddMinutes(3).TimeOfDay}\t\t\t\t\t\t RECHECKED BY:");
            writer.WriteLine($"\t File Rcvd:");
            writer.WriteLine($"\n\n");
            writer.WriteLine($"\f");
            writer.Write("\n \n");
        }
    }
}
