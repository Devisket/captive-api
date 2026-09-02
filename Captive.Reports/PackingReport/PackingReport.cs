using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Model.Dto.Reports;
using System.Text.RegularExpressions;

namespace Captive.Reports.PackingReport
{
    public class PackingReport : IPackingReport
    {
        // ---- Fixed-width layout (spaces only, no tabs) -------------------------------
        //          1         2         3         4         5         6         7
        // 01234567890123456789012345678901234567890123456789012345678901234567890123456789
        //         ACCT_NO         ACCOUNT_NAME                QTY CT START #    END #
        //         007-070-57616-0 ROYLYN ENRIQUEZ ASUNCION OR     1 A 0003319651 0003319700
        // -----------------------------------------------------------------------------
        private const string Indent = "        ";   // report starts at column 8
        private const int AccountNoWidth = 15;      // cols 8..22 (+1 space  -> name at col 24)
        private const int AccountNameWidth = 32;    // cols 24..55 (-> QTY at col 56)
        private const int NameColumn = 24;          // start column of the ACCOUNT_NAME field
        private const int PageWidth = 72;           // width used to centre the title lines

        private static readonly string ColumnHeader =
            Indent
            + "ACCT_NO".PadRight(16)        // cols 8..23
            + "ACCOUNT_NAME".PadRight(28)   // cols 24..51
            + "QTY".PadRight(4)             // cols 52..55
            + "CT".PadRight(3)              // cols 56..58
            + "START #".PadRight(11)        // cols 59..69
            + "END #";                      // cols 70..74

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
                    writer.NewLine = "\r\n";

                    var pageNo = 1;
                    foreach (var filBranch in orderFileBranchGroupBy.OrderBy(x => x.Key.BRSTNCode))
                    {
                        var subTotal = 0;
                        var firstData = filBranch.First();

                        RenderHeader(writer, formCheckName, productName, pageNo, firstData.BankBranch, filBranch.Key.OrderFileName, firstData.DeliverTo);

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

            if (!string.IsNullOrEmpty(accountNumberFormat))
                accNo = FormatAccountNumber(checkData.AccountNo, accountNumberFormat);

            // Second line rendered underneath AccountName1 (if any)
            var accountName = string.Empty;
            var overflowName = string.Empty;

            if (!String.IsNullOrEmpty(checkData.AccountName1))
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                accountName = regex.Replace(checkData.AccountName1, " ").Trim();

                if (!String.IsNullOrEmpty(checkData.AccountName2))
                {
                    // AccountName2 takes the second line; AccountName1 is cut at the column width
                    overflowName = regex.Replace(checkData.AccountName2, " ").Trim();
                }
                else if (accountName.Length > AccountNameWidth)
                {
                    // No AccountName2 - wrap the remainder of AccountName1 to the second line
                    overflowName = accountName.Substring(AccountNameWidth).Trim();
                }

                if (accountName.Length > AccountNameWidth)
                    accountName = accountName.Substring(0, AccountNameWidth);

                if (overflowName.Length > AccountNameWidth)
                    overflowName = overflowName.Substring(0, AccountNameWidth);
            }

            writer.WriteLine(
                Indent
                + accNo.PadRight(AccountNoWidth) + " "
                + accountName.PadRight(AccountNameWidth)
                + "1"
                + " " + formCheckType
                + " " + checkDto.StartSeries.PadLeft(10, '0')
                + " " + checkDto.EndSeries.PadLeft(10, '0'));

            if (!String.IsNullOrEmpty(overflowName))
            {
                // Align under the ACCOUNT_NAME column
                writer.WriteLine(new string(' ', NameColumn) + overflowName);
            }
        }

        private void RenderHeader(StreamWriter writer, string formCheckName, string productName, int pageNo, BankBranches orderBranch, string orderFileName, BankBranches? deliverTo)
        {
            var bankName = orderBranch.BankInfo.BankName;

            writer.WriteLine($"{Indent}Page No.{pageNo}");
            writer.WriteLine($"{Indent}{DateTime.UtcNow.ToString("dddd, dd MMMM yyyy")}");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine(Center("CAPTIVE PRINTING CORPORATION"));
            writer.WriteLine(Center($"{bankName} {productName} - {formCheckName} Checks Summary"));
            writer.WriteLine();
            writer.WriteLine(ColumnHeader);
            writer.WriteLine();
            writer.WriteLine();

            if (deliverTo != null)
            {
                writer.WriteLine($"{Indent}** DELIVER TO {deliverTo.BRSTNCode} {deliverTo.BranchName}");
                writer.WriteLine();
            }

            writer.WriteLine($"{Indent}** ORDERS OF BRSTN {orderBranch.BRSTNCode} {orderBranch.BranchName}({orderBranch.BranchCode ?? string.Empty})");
            writer.WriteLine();
            writer.WriteLine($"{Indent}* Batch #: {orderFileName.Split('.').First().ToUpper()}");
            writer.WriteLine();
        }

        private void RenderFooter(StreamWriter writer, int subTotal)
        {
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine($"{Indent}*** SUB TOTAL: {subTotal}");
            writer.WriteLine("\f");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine();
        }

        private static string Center(string text)
        {
            text = text.Trim();

            if (text.Length >= PageWidth)
                return text;

            return new string(' ', (PageWidth - text.Length) / 2) + text;
        }

        private string FormatAccountNumber(string accountNumber, string accountNumberFormat)
        {
            if (string.IsNullOrEmpty(accountNumberFormat))
                return accountNumber;

            var digits = new string(accountNumber.Where(char.IsDigit).ToArray());
            var result = new System.Text.StringBuilder();
            var digitIndex = 0;

            foreach (var ch in accountNumberFormat)
            {
                if (ch == '0')
                {
                    if (digitIndex < digits.Length)
                        result.Append(digits[digitIndex++]);
                }
                else
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }
    }
}
