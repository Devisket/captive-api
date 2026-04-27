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
                            writer.WriteLine($"        ** BLOCK {blockNo}");
                        }

                        RenderText(writer, checkOrder.CheckOrder, checkOrder.BankBranch, checkOrder.StartSeries, checkOrder.EndSeries, checkOrder.AccountNumberFormat);

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

        private void RenderText(StreamWriter writer, CheckOrders checkOrder, BankBranches branch, string? startingSeries, string? endingSeries, string? accountNumberFormat)
        {
            var accNo = checkOrder.AccountNo;

            if(!string.IsNullOrEmpty(accountNumberFormat))
                accNo = Regex.Replace(checkOrder.AccountNo, $"{accountNumberFormat}", @"$1-$2-$3");

            writer.WriteLine($"            {branch.BRSTNCode}    {accNo.PadRight(12)}    {startingSeries.PadLeft(10, '0')}    {endingSeries.PadLeft(10, '0')}");
        }

        private void RenderHeader(StreamWriter writer, string bankShortName, string productDescription, string formCheckDescription, int page)
        {
            writer.WriteLine();
            writer.WriteLine($"         Page No. {page}");
            writer.WriteLine($"         {DateTime.Now.ToString("MM/dd/yyyy")}");
            writer.WriteLine();
            writer.WriteLine($"                         {bankShortName.ToUpper()} - SUMMARY OF BLOCK - {formCheckDescription.ToUpper()}");
            writer.WriteLine($"                        {productDescription.ToUpper()}");
            writer.WriteLine();
            writer.WriteLine("            BLOCK RT_NO  ACCT_NO         START_NO.     END_NO.");
            writer.WriteLine();
        }

        private void RenderFooter(StreamWriter writer, List<Tuple<string, int>> formcheckType, string fileName, DateTime deliveryDate)
        {
            writer.WriteLine();
            bool isFirst = true;
            foreach (var item in formcheckType)
            {
                if (isFirst)
                {
                    writer.WriteLine($"        {item.Item1} = {item.Item2}                         {fileName}                                       DLVR: {deliveryDate:MM-dd}({deliveryDate:ddd})");
                    isFirst = false;
                }
                else
                {
                    writer.WriteLine($"        {item.Item1} = {item.Item2}");
                }
            }

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine($"        Prepared By  :                                   RECHECKED By :   ");
            writer.WriteLine($"        Updated By   :  ");
            writer.WriteLine($"        Time Start   :  {DateTime.Now:HH:mm}");
            writer.WriteLine($"        Time Finished:  {DateTime.Now.AddMinutes(3):HH:mm}");
            writer.WriteLine($"        File Recvd   :  ");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("\f");
        }
    }
}
