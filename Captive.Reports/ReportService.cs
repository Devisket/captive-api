using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto.Reports;
using Microsoft.EntityFrameworkCore;

namespace Captive.Reports
{
    public interface IReportService
    {
        Task<ICollection<CheckOrderReport>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankId, CancellationToken cancellationToken);
        Task<ICollection<BankBranches>> GetAlLBranches(Guid bankId, CancellationToken cancellationToken);
        Task<ICollection<CheckInventoryDetail>> GetCheckInventory(Guid checkOrderId, CancellationToken cancellationToken);

    }

    public class ReportService : IReportService
    {
        private IReadUnitOfWork _readUow;

        public ReportService(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<ICollection<CheckOrderReport>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankId, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(bankId, cancellationToken);

            var returnDatas = new List<CheckOrderReport>();

            var formChecks = await GetFormChecks(checkOrders.GroupBy(x => x.FormCheckId ?? Guid.Empty).Select(x => x.Key).ToList());

            foreach (var checkOrder in checkOrders)
            {
                var checkInventory = await GetCheckInventory(checkOrder.Id, cancellationToken);

                var branch = branches.First(x => x.BRSTNCode == checkOrder.BRSTN);

                var formCheck = formChecks.First(x => x.Id == checkOrder.FormCheckId);

                foreach (var check in checkInventory)
                {
                    returnDatas.Add(new CheckOrderReport
                    {
                        ProductTypeName = formCheck.Product.ProductName,
                        CustomizeFileName = formCheck.Product.CustomizeFileName,
                        FormCheckName = formCheck.Description,
                        FileInitial = formCheck.FileInitial,
                        CheckType = formCheck.CheckType,
                        FormType = formCheck.FormType,
                        CheckOrder = checkOrder,
                        FormCheckType = formCheck.FormCheckType,
                        BankBranch = branch,
                        NoOfPadding = check.CheckInventory!.NumberOfPadding,
                        CheckInventoryId = check.Id,
                        BarcodeValue = check.BarCodeValue,
                        OrderFileName = checkOrder.OrderFile.FileName,
                        StartSeries = check.StartingSeries ?? string.Empty,
                        EndSeries = check.EndingSeries ?? string.Empty,
                        SeriesPattern = check.CheckInventory!.SeriesPatern,
                        AccountNumberFormat = branches.First().BankInfo.AccountNumberFormat,
                    });
                }
            }

            return returnDatas;
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
                .Include(x => x.CheckInventory)
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
    }
}
