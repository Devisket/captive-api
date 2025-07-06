using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Reports.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Captive.Reports.BlockReport
{
    public  class BlockReport : IBlockReport
    {
        private IReadUnitOfWork _readUow;

        public BlockReport(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(batchFile.BankInfoId, cancellationToken);

            var checkDto = await ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName });

            int runningNo = 0, blockNo = 0, pageNo = 0;


            foreach (var productCheckOrder in productGroup)
            {
                var productName = productCheckOrder.Key.ProductTypeName;
                var formCheckName = productCheckOrder.Key.FormCheckName ?? string.Empty;
                var bankShortName = branches.First().BankInfo.ShortName;
                var fileName = productCheckOrder.First().OrderFileName;

                var productFCGroup = checkDto.Where(x => x.ProductTypeName == productCheckOrder.Key.ProductTypeName).GroupBy(x => x.FormCheckName);

                List<Tuple<string, int>> formcheckList = new List<Tuple<string, int>>();
                foreach (var productFC in productFCGroup)
                {
                    formcheckList.Add(new Tuple<string, int>(productFC.First().FileInitial, productFC.Count()));
                }

                var productFilePath = Path.Combine(filePath, productCheckOrder.Key.ProductTypeName, $"Block{formCheckName?.First()}.txt");

                using (StreamWriter writer = new StreamWriter(productFilePath, true))
                {
                    foreach (var checkOrder in productCheckOrder.OrderBy(x => x.BankBranch.Id).ThenBy(x => x.StartSeries))
                    {
                        if ((blockNo % 8) == 0 && (runningNo % 4) == 0)
                        {
                            pageNo++;
                            RenderHeader(writer, bankShortName, productName, formCheckName, pageNo);
                        }

                        if ((runningNo % 4) == 0 || blockNo == 0)
                        {
                            blockNo++;
                            writer.WriteLine($" ** BLOCK{blockNo}");
                        }

                        RenderText(writer, checkOrder.CheckOrder, checkOrder.BankBranch, checkOrder.StartSeries, checkOrder.EndSeries, blockNo);

                        runningNo++;

                        if ((blockNo % 8) == 0 && (runningNo % 4) == 0 && pageNo == 1)
                            RenderFooter(writer, formcheckList, fileName);
                    }

                    if (blockNo <= 4)
                        RenderFooter(writer, formcheckList,fileName);

                    runningNo = 0;
                    blockNo = 0;
                    pageNo = 0;
                }
            }
        }

        private void RenderText(StreamWriter writer, CheckOrders checkOrder, BankBranches branch, string? startingSeries, string? endingSeries, int blockNo)
        {
            var accNo = Regex.Replace(checkOrder.AccountNo, @"(\w{3})(\w{6})(\w{3})", @"$1-$2-$3");
            writer.WriteLine($"\t  {blockNo} {branch.BRSTNCode}\t{accNo}\t{startingSeries.PadLeft(10, '0')} {endingSeries.PadLeft(10,'0')}");
        }

        private void RenderHeader(StreamWriter writer, string bankShortName, string productDescription, string formCheckDescription, int page)
        {
            writer.WriteLine();
            writer.WriteLine($"\t \t Page No.{page}");
            writer.WriteLine($"\t \t {DateTime.Now.ToString("ddd, dd MMMM yyyy")}");
            writer.WriteLine($"\t \t \t \t  {bankShortName.ToUpper()} - SUMMARY OF BLOCK - {formCheckDescription.ToUpper()} Check");
            writer.WriteLine($"\t \t \t \t \t \t \t \t    {productDescription.ToUpper()} Check");
            writer.Write("\n \n");
            writer.WriteLine("  BLOCK RT_NO\t\tACCT_NO\t\t\tSTART_NO.\tEND_NO.\t\tDELIVER_TO");
        }

        private void RenderFooter(StreamWriter writer, List<Tuple<string, int>> formcheckType, string fileName)
        {
            writer.WriteLine();
            foreach (var item in formcheckType)
            {

                writer.Write($"\t {item.Item1}: {item.Item2}");

                if (item.Equals(formcheckType.Last()))
                    writer.Write($"\t\t\t\t\t\t {fileName}");

                writer.Write('\n');
            }

            writer.WriteLine($"\t Prepared By:");
            writer.WriteLine($"\t Updated By:");
            writer.WriteLine($"\t Time Start:");
            writer.WriteLine($"\t Time Finished:\t\t\t\t\t\t RECHECKED BY:");
            writer.WriteLine($"\t File Rcvd:");
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

        private async Task<ICollection<CheckOrderDTO>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankId, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(bankId, cancellationToken);

            var returnDatas = new List<CheckOrderDTO>();

            var formChecks = await GetFormChecks(checkOrders.GroupBy(x => x.FormCheckId ?? Guid.Empty).Select(x => x.Key).ToList());

            foreach (var checkOrder in checkOrders)
            {
                var checkInventory = await GetCheckInventory(checkOrder.Id, cancellationToken);

                var branch = branches.First(x => x.BRSTNCode == checkOrder.BRSTN);

                var formCheck = formChecks.First(x => x.Id == checkOrder.FormCheckId);

                foreach (var check in checkInventory)
                {
                    returnDatas.Add(new CheckOrderDTO
                    {
                        ProductTypeName = formCheck.Product.ProductName,
                        FormCheckName = formCheck.Description,
                        FileInitial = formCheck.FileInitial,
                        CheckOrder = checkOrder,
                        BankBranch = branch,
                        OrderFileName = checkOrder.OrderFile.FileName,
                        CheckInventoryId = check.Id,
                        StartSeries = check.StartingSeries ?? string.Empty,
                        EndSeries = check.EndingSeries ?? string.Empty
                    });
                }
            }

            return returnDatas;
        }
    }
}
