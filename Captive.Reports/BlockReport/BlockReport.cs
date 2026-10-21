using System.Text.RegularExpressions;
using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto.Reports;

namespace Captive.Reports.BlockReport
{
    public class BlockReport : IBlockReport
    {
        private IReportService _reportService;

        public BlockReport(IReadUnitOfWork readUow, IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task GenerateReport(
            BatchFile batchFile,
            ICollection<CheckOrderReport> checkDto,
            string filePath,
            CancellationToken cancellationToken
        )
        {
            if (!checkDto.Any())
                return;

            var branches = await _reportService.GetAlLBranches(
                batchFile.BankInfoId,
                cancellationToken
            );

            // FileSuffix is only set when the order file uses split generation (BlockC1, BlockC2, ...)
            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName, x.FileSuffix });

            var formCheckTypeCount = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckType, x.FileSuffix });

            int runningNo = 0,
                blockNo = 0,
                pageNo = 0;

            foreach (var productCheckOrder in productGroup)
            {
                var productName = productCheckOrder.Key.ProductTypeName;
                var formCheckName = productCheckOrder.Key.FormCheckName ?? string.Empty;
                var fileSuffix = productCheckOrder.Key.FileSuffix;
                var bankShortName = branches.First().BankInfo.ShortName;
                var fileName = productCheckOrder.First().OrderFileName.Split('.').First();

                List<Tuple<string, int>> formcheckList = new List<Tuple<string, int>>();

                foreach (var checkType in formCheckTypeCount.Where(x => x.Key.ProductTypeName == productName && x.Key.FileSuffix == fileSuffix))
                {
                    var checkTypeInitial = checkType.Key.FormCheckType == FormCheckType.Personal ? "A" : "B";

                    formcheckList.Add(
                        new Tuple<string, int>(
                            checkTypeInitial,
                            checkType.Sum(x => x.FormCheckQuantity)
                        )
                    );
                }

                var productFilePath = Path.Combine(
                    filePath,
                    productCheckOrder.Key.ProductTypeName,
                    $"Block{formCheckName?.First()}{fileSuffix}.txt"
                );

                using (StreamWriter writer = new StreamWriter(productFilePath, true))
                {
                    var printerFileName =
                        $"{productCheckOrder.First().CustomizeFileName}{batchFile.DeliveryDate.Month}{batchFile.DeliveryDate.Day}{productCheckOrder.Key.FormCheckName!.First()}{fileSuffix}";

                    foreach (
                        var checkOrder in productCheckOrder
                            .OrderBy(x => x.BankBranch.BRSTNCode)
                            .ThenBy(x => x.CheckOrder.AccountNo)
                            .ThenBy(x => x.StartSeries)
                    )
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

                        RenderText(
                            writer,
                            checkOrder.CheckOrder,
                            checkOrder.BankBranch,
                            checkOrder.StartSeries,
                            checkOrder.EndSeries,
                            checkOrder.AccountNumberFormat
                        );

                        runningNo++;

                        if (
                            (
                                (blockNo % 8) == 0 && (runningNo % 4) == 0
                                || (
                                    checkOrder
                                    == productCheckOrder
                                        .OrderBy(x => x.BankBranch.BRSTNCode)
                                        .ThenBy(x => x.CheckOrder.AccountNo)
                                        .ThenBy(x => x.StartSeries)
                                        .Last()
                                )
                            )
                            && pageNo == 1
                        )
                            RenderFooter(
                                writer,
                                formcheckList,
                                fileName,
                                printerFileName,
                                batchFile.DeliveryDate
                            );
                    }
                    Console.WriteLine();

                    if (blockNo <= 4 && pageNo == 1)
                        RenderFooter(
                            writer,
                            formcheckList,
                            fileName,
                            printerFileName,
                            batchFile.DeliveryDate
                        );

                    runningNo = 0;
                    blockNo = 0;
                    pageNo = 0;
                }
            }
        }

        private void RenderText(
            StreamWriter writer,
            CheckOrders checkOrder,
            BankBranches branch,
            string? startingSeries,
            string? endingSeries,
            string? accountNumberFormat
        )
        {
            var accNo = checkOrder.AccountNo;

            if (!string.IsNullOrEmpty(accountNumberFormat))
                accNo = Regex.Replace(checkOrder.AccountNo, $"{accountNumberFormat}", @"$1-$2-$3");

            writer.WriteLine(
                $"            {branch.BRSTNCode}    {accNo.PadRight(12)}    {startingSeries.PadLeft(10, '0')}    {endingSeries.PadLeft(10, '0')}"
            );
        }

        private void RenderHeader(
            StreamWriter writer,
            string bankShortName,
            string productDescription,
            string formCheckDescription,
            int page
        )
        {
            writer.WriteLine();
            writer.WriteLine($"         Page No. {page}");
            writer.WriteLine($"         {DateTime.Now.ToString("MM/dd/yyyy")}");
            writer.WriteLine();
            writer.WriteLine(
                $"                         {bankShortName.ToUpper()} - SUMMARY OF BLOCK - {formCheckDescription.ToUpper()}"
            );
            writer.WriteLine($"                        {productDescription.ToUpper()}");
            writer.WriteLine();
            writer.WriteLine("            BLOCK RT_NO  ACCT_NO         START_NO.     END_NO.");
            writer.WriteLine();
        }

        private void RenderFooter(
            StreamWriter writer,
            List<Tuple<string, int>> formcheckType,
            string fileName,
            string printerFileName,
            DateTime deliveryDate
        )
        {
            writer.WriteLine();
            bool isFirst = true;
            foreach (var item in formcheckType)
            {
                if (isFirst)
                {
                    writer.WriteLine(
                        $"        {item.Item1} = {item.Item2}              {printerFileName}                         {fileName}                                     DLVR: {deliveryDate:MM-dd}({deliveryDate:ddd})"
                    );
                    isFirst = false;
                }
                else
                {
                    writer.WriteLine($"        {item.Item1} = {item.Item2}");
                }
            }

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine(
                $"        Prepared By  :                                   RECHECKED By :   "
            );
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
