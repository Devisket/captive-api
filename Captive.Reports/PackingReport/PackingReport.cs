using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Model.Dto.Reports;
using System.Text.RegularExpressions;

namespace Captive.Reports.PackingReport
{
    public class PackingReport : IPackingReport
    {
        private readonly IReportService _reportService;

        public PackingReport(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var checkDto = await _reportService.ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName, x.FormCheckType });

            var accountNumberFormat = checkDto.First().AccountNumberFormat;

            foreach (var productData in productGroup)
            {
                var productName = productData.Key.ProductTypeName;
                var formCheckName = productData.Key.FormCheckName ?? string.Empty;

                var formCheckType = productData.Key.FormCheckType;
                
                var initialFileName = formCheckType == Data.Enums.FormCheckType.Personal ? "A" : "B";

                var productFilePath = Path.Combine(filePath, productData.Key.ProductTypeName, $"Packing{initialFileName}.txt");

                var orderFileBranchGroupBy = productData.GroupBy(x => new { x.BankBranch.BRSTNCode, DeliveryBrstn = x.DeliverTo == null ? string.Empty : x.DeliverTo.BRSTNCode, x.OrderFileName });

                using (StreamWriter writer = new StreamWriter(productFilePath, true))
                {
                    var pageNo = 1;
                    foreach (var filBranch in orderFileBranchGroupBy.OrderBy(x => x.Key.BRSTNCode))
                    {
                        var subTotal = 0;
                        var firstData = filBranch.First();

                        RenderHeader(writer, formCheckName, pageNo, firstData.BankBranch, filBranch.Key.OrderFileName, firstData.DeliverTo);

                        foreach (var checkOrder in filBranch.OrderBy(x => x.BankBranch.BRSTNCode).ThenBy(x => x.CheckOrder.AccountNo).ThenBy(x => x.StartSeries))
                        {
                            RenderData(writer, checkOrder, accountNumberFormat);
                            subTotal++;
                        }

                        RenderFooter(writer, subTotal);
                        subTotal = 0;
                        pageNo++;
                    }
                }
            }
        }

        private void RenderData(StreamWriter writer, CheckOrderReport checkDto, string? accountNumberFormat)
        {
            var checkData = checkDto.CheckOrder;
            var formCheckType = checkDto.FormCheckType == FormCheckType.Personal ? "A" : "B";

            var accNo = checkData.AccountNo;

            if(!string.IsNullOrEmpty(accountNumberFormat))
                accNo = FormatAccountNumber(checkData.AccountNo, accountNumberFormat);

            writer.Write($"  {accNo}");

            if (!String.IsNullOrEmpty(checkData.AccountName))
            {

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                var accountName = regex.Replace(checkData.AccountName, " ");

                // Ensure account name is exactly 50 characters
                if (accountName.Length > 50)
                {
                    accountName = accountName.Substring(0, 50);
                }
                accountName = accountName.PadRight(50);

                writer.Write($"  {accountName}");
            }
            else
                writer.Write($"  {new string(' ', 50)}");

            writer.Write("  \t1");
            writer.Write($"\t{formCheckType}");
            writer.Write($"\t{checkDto.StartSeries.PadLeft(10,'0')}  \t{checkDto.EndSeries.PadLeft(10,'0')}\n");
        }

        private void RenderHeader(StreamWriter writer, string formCheckName, int pageNo, BankBranches orderBranch, string orderFileName, BankBranches? deliverTo)
        {
            var bankShortName = orderBranch.BankInfo.ShortName;

            writer.WriteLine($"  Page No.{pageNo}");
            writer.WriteLine($"  {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy")}");
            writer.WriteLine("\t\t\t\t\t\t\t  CAPTIVE PRINTING CORPORATION");
            writer.WriteLine($"\t\t\t\t\t\t\t  {bankShortName} - {formCheckName} Summary");
            writer.WriteLine($"  ACCT_NO \t\t  ACCOUNT_NAME \t\t\t\t\tQTY\tCT\tSTART #\t\tEND #");

            if (deliverTo != null)
                writer.WriteLine($"\n ** DELIVER TO {deliverTo.BRSTNCode} {deliverTo.BranchName}");

            writer.WriteLine($"\n ** ORDERS OF BRSTN {orderBranch.BRSTNCode} {orderBranch.BranchName}({orderBranch.BranchCode ?? string.Empty})");
            writer.WriteLine($"\n * Batch #: {orderFileName.Split('.').First().ToUpper()} \n");
        }
        private void RenderFooter(StreamWriter writer, int subTotal)
        {
            writer.WriteLine($"\n *** SUB TOTAL: {subTotal}\n");
            writer.WriteLine("\f");
        }

        private string FormatAccountNumber(string accountNumber, string format)
        {
            if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(format))
                return accountNumber;

            var segments = format.Split('-');
            var result = new List<string>();
            var currentIndex = 0;

            foreach (var segment in segments)
            {
                var segmentLength = segment.Length;
                if (currentIndex + segmentLength <= accountNumber.Length)
                {
                    result.Add(accountNumber.Substring(currentIndex, segmentLength));
                    currentIndex += segmentLength;
                }
                else if (currentIndex < accountNumber.Length)
                {
                    result.Add(accountNumber.Substring(currentIndex));
                    break;
                }
            }

            return string.Join("-", result);
        }
    }
}
