using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto.Reports;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Captive.Reports.PrinterFileReport
{
    public class PrinterFileReport : IPrinterFileReport
    {
        private readonly IReadUnitOfWork _readUow;
        public PrinterFileReport(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(batchFile.BankInfoId);

            var checkDto = await ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId);

            var productGroup = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName });

            foreach (var productCheckOrder in productGroup)
            {
                var productName = productCheckOrder.Key.ProductTypeName;
                var productFilePath = Path.Combine(filePath, productCheckOrder.Key.ProductTypeName, $"PrinterFile{productCheckOrder.Key.FormCheckName.First()}.txt");

                using (StreamWriter writer = new StreamWriter(productFilePath, true))
                {
                    foreach (var checkOrder in productCheckOrder.OrderBy(x => x.BankBranch.BRSTNCode).ThenBy(x => x.CheckOrder.AccountNo).ThenBy(x => x.StartSeries))
                    {
                        RenderText(writer, checkOrder.CheckOrder, checkOrder.BankBranch, checkOrder.StartSeries, checkOrder.EndSeries, checkOrder.CheckType);
                    }
                }
            }
        }

        private void RenderText(StreamWriter writer, CheckOrders checkOrder, BankBranches branch, string? startingSeries, string? endingSeries, string CheckType)
        {
            var concodes = string.IsNullOrEmpty(checkOrder.Concode) ? null : checkOrder.Concode.Split(";");

            var barcodeValues = !string.IsNullOrEmpty(checkOrder.BarCodeValue) ? checkOrder.BarCodeValue.Split(';') : new string[] { };

            writer.WriteLine(3);
            writer.WriteLine(checkOrder.BRSTN);
            writer.WriteLine(checkOrder.AccountNo);
            //Todo endingseries + 1
            writer.WriteLine(endingSeries);
            writer.WriteLine(CheckType);
            writer.WriteLine();
            writer.WriteLine(checkOrder.BRSTN.Substring(0, 5));
            writer.WriteLine(string.Format(" {0}", checkOrder.BRSTN.Substring(4, 4)));

            var formattedAccNo = Regex.Replace(checkOrder.AccountNo, @"(\w{3})(\w{6})(\w{3})", @"$1-$2-$3");
            writer.WriteLine(formattedAccNo);
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

        private async Task<ICollection<CheckInventoryDetail>> GetCheckInventory(Guid checkOrderId)
        {
            var checkInventory = await _readUow.CheckInventoryDetails.GetAll()
                .AsNoTracking()
                .Where(x => x.CheckOrderId == checkOrderId)
                .ToListAsync();

            return checkInventory;
        }

        private async Task<ICollection<BankBranches>> GetAlLBranches(Guid bankId)
        {
            var bankBranches = await _readUow.BankBranches.GetAll()
                .Include(x => x.BankInfo)
                .Where(x => x.BankInfoId == bankId)
                .AsNoTracking()
                .ToListAsync();

            return bankBranches;
        }

        private async Task<ICollection<FormChecks>> GetFormChecks (List<Guid> formCheckIds)
        {
            var formCheck = await _readUow.FormChecks.GetAll()
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => formCheckIds.Any(z => z == x.Id))
                .AsNoTracking()
                .ToListAsync();

            return formCheck;
        }

        private async Task<ICollection<CheckOrderReport>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankID)
        {
            var branches = await GetAlLBranches(bankID);

            var returnDatas = new List<CheckOrderReport>();

            var formChecks = await GetFormChecks(checkOrders.GroupBy(x => x.FormCheckId ?? Guid.Empty).Select(x => x.Key).ToList());

            foreach (var checkOrder in checkOrders)
            {
                var checkInventory = await GetCheckInventory(checkOrder.Id);

                var branch = branches.First(x => x.Id == checkOrder.BranchId);

                var formCheck = formChecks.First(x => x.Id == checkOrder.FormCheckId);

                foreach (var check in checkInventory)
                {
                    returnDatas.Add(new CheckOrderReport
                    {
                        ProductTypeName = formCheck.Product.ProductName,
                        FormCheckName = formCheck.Description,
                        FileInitial = formCheck.FileInitial,
                        CheckType = formCheck.CheckType,
                        FormType = formCheck.FormType,
                        CheckOrder = checkOrder,
                        FormCheckType = formCheck.FormCheckType,
                        BankBranch = branch,
                        CheckInventoryId = check.Id,
                        StartSeries = check.StartingSeries ?? string.Empty,
                        EndSeries = check.EndingSeries ?? string.Empty,
                        AccountNumberFormat = branch.BankInfo.AccountNumberFormat
                    });
                }
            }

            return returnDatas;
        }
    }
}
