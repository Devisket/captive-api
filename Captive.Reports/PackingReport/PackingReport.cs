using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Reports.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName });

            foreach(var productData in productGroup)
            {
                var productName = productData.Key.ProductTypeName;
                var formCheckName = productData.Key.FormCheckName ?? string.Empty;


                var productFilePath = Path.Combine(filePath, productData.Key.ProductTypeName, $"Packing{formCheckName?.First()}.txt");

                var orderFileBranchGroupBy = productData.GroupBy(x => new { x.BankBranch.BRSTNCode, DeliveryBrstn = x.DeliverTo == null ? string.Empty : x.DeliverTo.BRSTNCode, x.OrderFileName });

                using(StreamWriter writer = new StreamWriter(productFilePath,true))
                {
                    var pageNo = 1;
                    foreach (var filBranch in orderFileBranchGroupBy)
                    {
                        var subTotal = 0;
                        var firstData = filBranch.First();

                        RenderHeader(writer, formCheckName, pageNo, firstData.BankBranch, filBranch.Key.OrderFileName, firstData.DeliverTo);

                        foreach (var checkOrder in filBranch)
                        {
                            RenderData(writer, checkOrder);
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
                .Where(x => x.BankId == bankId)
                .AsNoTracking()
                .ToListAsync();

            return bankBranches;
        }

        public async Task<ICollection<CheckInventory>> GetCheckInventory(Guid checkOrderId, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory.GetAll()
               .AsNoTracking()
               .Where(x => x.CheckOrderId == checkOrderId)
               .ToListAsync();

            return checkInventory;
        }

        private async Task<ICollection<CheckOrderDTO>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankId, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(bankId, cancellationToken);

            var returnDatas = new List<CheckOrderDTO>();

            foreach (var checkOrder in checkOrders)
            {
                var checkInventory = await GetCheckInventory(checkOrder.Id, cancellationToken);

                var branch = branches.First(x => x.BRSTNCode == checkOrder.BRSTN);

                var deliveringBranch = branches.FirstOrDefault(x => x.BRSTNCode == checkOrder.DeliverTo);

                foreach (var check in checkInventory)
                {
                    returnDatas.Add(new CheckOrderDTO
                    {
                        ProductTypeName = checkOrder.FormChecks.ProductType.ProductName,
                        FormCheckName = checkOrder.FormChecks.Description,
                        FileInitial = checkOrder.FormChecks.FileInitial,
                        CheckOrder = checkOrder,
                        BankBranch = branch,
                        DeliverTo = deliveringBranch,
                        CheckInventoryId = check.Id,
                        StartSeries = check.StarSeries ?? string.Empty,
                        EndSeries = check.EndSeries ?? string.Empty,
                        OrderFileId = checkOrder.OrderFileId,
                        OrderFileName =  checkOrder.OrderFile.FileName
                    });
                }
            }

            return returnDatas;
        }


        private void RenderData(StreamWriter writer, CheckOrderDTO checkDto)
        {
            var checkData = checkDto.CheckOrder;

            var accNo = Regex.Replace(checkData.AccountNo, @"(\w{3})(\w{6})(\w{3})", @"$1-$2-$3");

            writer.Write($"  {accNo}");
            
            if (!String.IsNullOrEmpty(checkData.AccountName)) 
            {

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                var accountName = regex.Replace(checkData.AccountName, " ");
                                
                writer.Write($"  {string.Format("{0,-38}", accountName)}");
            }
            else
                writer.Write($"  \t\t\t\t\t\t\t\t\t\t");

            writer.Write("1  ");
            writer.Write($"{checkDto.StartSeries}  {checkDto.EndSeries}\n");

        }

        private void RenderHeader(StreamWriter writer, string formCheckName, int pageNo, BankBranches orderBranch, string orderFileName, BankBranches? deliverTo) 
        {
            var bankShortName = orderBranch.BankInfo.ShortName;

            writer.WriteLine($"  Page No.{pageNo}");
            writer.WriteLine($"  {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy")}");
            writer.WriteLine("\t\t\t\t\t\t\t  CAPTIVE PRINTING CORPORATION");
            writer.WriteLine($"\t\t\t\t\t\t\t  {bankShortName} - {formCheckName} Summary");
            writer.WriteLine($"  ACCT_NO \t\t  ACCOUNT_NAME \t\t\t\t\t\t  QTY\tSTART #\t  END #");

            if(deliverTo != null)
                writer.WriteLine($"\n ** DELIVER TO {deliverTo.BRSTNCode} {deliverTo.BranchName}");

            writer.WriteLine($"\n ** ORDERS OF BRSTN {orderBranch.BRSTNCode} {orderBranch.BranchName}");
            writer.WriteLine($"\n * Batch #: {orderFileName.ToUpper()} \n");
        }
        private void RenderFooter(StreamWriter writer, int subTotal) {

            writer.WriteLine($"\n *** SUB TOTAL: {subTotal}\n");
        }
    }
}
