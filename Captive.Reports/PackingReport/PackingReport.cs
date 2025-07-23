using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto.Reports;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Captive.Reports.PackingReport
{
    public class PackingReport : IPackingReport
    {
        private IReadUnitOfWork _readUow;

        public PackingReport(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var checkDto = await ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

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
                    foreach (var filBranch in orderFileBranchGroupBy)
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

        public async Task<ICollection<BankBranches>> GetAlLBranches(Guid bankId, CancellationToken cancellationToken)
        {
            var bankBranches = await _readUow.BankBranches.GetAll()
                .Include(x => x.BankInfo)
                .Where(x => x.BankInfoId == bankId)
                .AsNoTracking()
                .ToListAsync();

            return bankBranches;
        }

        public async Task<ICollection<CheckInventoryDetail>> GetCheckInventory(Guid checkOrderId, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventoryDetails.GetAll()
               .AsNoTracking()
               .Where(x => x.CheckOrderId == checkOrderId)
               .ToListAsync();

            return checkInventory;
        }

        private async Task<ICollection<FormChecks>> GetFormChecks(List<Guid> formCheckIds)
        {
            var formCheck = await _readUow.FormChecks.GetAll()
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => formCheckIds.Any(z => z == x.Id))
                .AsNoTracking()
                .ToListAsync();

            return formCheck;
        }

        private async Task<ICollection<CheckOrderReport>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankId, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(bankId, cancellationToken);

            var formChecks = await GetFormChecks(checkOrders.GroupBy(x => x.FormCheckId ?? Guid.Empty).Select(x => x.Key).ToList());

            var returnDatas = new List<CheckOrderReport>();

            foreach (var checkOrder in checkOrders)
            {
                var checkInventory = await GetCheckInventory(checkOrder.Id, cancellationToken);

                var branch = branches.First(x => x.BRSTNCode == checkOrder.BRSTN);

                var deliveringBranch = branches.FirstOrDefault(x => x.BRSTNCode == checkOrder.DeliverTo);

                var formCheck = formChecks.First(x => x.Id == checkOrder.FormCheckId);

                foreach (var check in checkInventory)
                {
                    returnDatas.Add(new CheckOrderReport
                    {
                        ProductTypeName = formCheck.Product.ProductName,
                        FormCheckName = formCheck.Description,
                        FileInitial = formCheck.FileInitial,
                        FormCheckType = formCheck.FormCheckType,
                        CheckOrder = checkOrder,
                        BankBranch = branch,
                        DeliverTo = deliveringBranch,
                        CheckInventoryId = check.Id,
                        StartSeries = check.StartingSeries ?? string.Empty,
                        EndSeries = check.EndingSeries ?? string.Empty,
                        BarcodeValue = check.BarCodeValue,
                        OrderFileId = checkOrder.OrderFileId,
                        OrderFileName = checkOrder.OrderFile.FileName
                    });
                }
            }
            return returnDatas;
        }

        private void RenderData(StreamWriter writer, CheckOrderReport checkDto, string? accountNumberFormat)
        {
            var checkData = checkDto.CheckOrder;

            var accNo = checkData.AccountNo;

            if(!string.IsNullOrEmpty(accountNumberFormat))
                accNo = Regex.Replace(checkData.AccountNo, $"{accountNumberFormat}", @"$1-$2-$3");

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
            writer.Write($"\t{checkDto.StartSeries.PadLeft(10,'0')}  \t{checkDto.EndSeries.PadLeft(10,'0')}\n");
        }

        private void RenderHeader(StreamWriter writer, string formCheckName, int pageNo, BankBranches orderBranch, string orderFileName, BankBranches? deliverTo)
        {
            var bankShortName = orderBranch.BankInfo.ShortName;

            writer.WriteLine($"  Page No.{pageNo}");
            writer.WriteLine($"  {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy")}");
            writer.WriteLine("\t\t\t\t\t\t\t  CAPTIVE PRINTING CORPORATION");
            writer.WriteLine($"\t\t\t\t\t\t\t  {bankShortName} - {formCheckName} Summary");
            writer.WriteLine($"  ACCT_NO \t\t  ACCOUNT_NAME \t\t\t\t\tQTY\tSTART #\t\tEND #");

            if (deliverTo != null)
                writer.WriteLine($"\n ** DELIVER TO {deliverTo.BRSTNCode} {deliverTo.BranchName}");

            writer.WriteLine($"\n ** ORDERS OF BRSTN {orderBranch.BRSTNCode} {orderBranch.BranchName}");
            writer.WriteLine($"\n * Batch #: {orderFileName.Split('.').First().ToUpper()} \n");
        }
        private void RenderFooter(StreamWriter writer, int subTotal)
        {
            writer.WriteLine($"\n *** SUB TOTAL: {subTotal}\n");
            writer.WriteLine("\f");
        }
    }
}
