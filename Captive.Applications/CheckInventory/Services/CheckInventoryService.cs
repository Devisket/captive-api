using Captive.Applications.CheckValidation.Services;
using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Services
{
    public interface ICheckInventoryService
    {
        Task<LogDto> ApplyCheckInventory(OrderFile orderFile, CancellationToken cancellationToken);
    }

    public class CheckInventoryService : ICheckInventoryService
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckValidationService _checkValidationService;
        private readonly IStringService _stringService;

        public CheckInventoryService(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, ICheckValidationService checkValidationService, IStringService stringService)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkValidationService = checkValidationService;
            _stringService = stringService;
        }

        public async Task<LogDto> ApplyCheckInventory(OrderFile orderFile, CancellationToken cancellationToken)
        {
            var logDto = new LogDto { };

            var formCheck = await _readUow.FormChecks.GetAll()
                .AsNoTracking()
                .FirstAsync(x => x.ProductId == orderFile.ProductId, cancellationToken);

            var bankId = orderFile.BatchFile!.BankInfoId;

            var checkOrders = _readUow.CheckOrders.GetAllLocal()
                .Where(x => x.OrderFileId == orderFile.Id)
                .OrderBy(x => x.AccountNo)
                .ToArray();

            foreach (var checkOrder in checkOrders)
            {
                var orderFormCheck = await _readUow.FormChecks.GetAll().AsNoTracking().FirstAsync(x => x.Id == checkOrder.FormCheckId, cancellationToken);

                var checkInventory = await _checkValidationService.GetCheckInventoryDirect(
                    bankId,
                    checkOrder.BranchId,
                    checkOrder.ProductId,
                    orderFormCheck.FormCheckType,
                    checkOrder.AccountNo,
                    cancellationToken);

                if (!string.IsNullOrEmpty(checkOrder.PreStartingSeries) && !string.IsNullOrEmpty(checkOrder.PreEndingSeries))
                {
                    await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
                    {
                        Id = Guid.Empty,
                        ProductId = orderFile.ProductId,
                        CheckOrderId = checkOrder.Id,
                        StartingSeries = checkOrder.PreStartingSeries,
                        EndingSeries = checkOrder.PreEndingSeries,
                        CheckInventoryId = checkInventory!.Id,
                        Quantity = orderFormCheck!.Quantity,
                        BranchId = checkOrder.BranchId,
                        AccountNumber = checkOrder.AccountNo,
                        FormCheckId = orderFormCheck.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    }, cancellationToken);

                    continue;
                }

                var startingSeriesNumber = checkInventory.StartingSeries + 1;

                var lastCheck = GetLastCheckDetail(checkInventory, checkOrder);

                if (lastCheck != null)
                    startingSeriesNumber = lastCheck.EndingNumber + 1;

                var endingSeriesNumber = (startingSeriesNumber + orderFormCheck.Quantity) - 1;

                for (int i = 1; i <= checkOrder.Quantity; i++)
                {
                    var series = _stringService.ConvertToSeries(checkInventory!.SeriesPatern, checkInventory.NumberOfPadding, startingSeriesNumber, endingSeriesNumber);

                    var warningMsg = _checkValidationService.HitWarningSeries(checkInventory, series.Item1, series.Item2);

                    if (!string.IsNullOrEmpty(warningMsg))
                    {
                        logDto.LogType = Model.Enums.LogType.Warning;
                        logDto.LogMessage = warningMsg;
                    }

                    if (await _checkValidationService.HasConflictedSeries(series.Item1, series.Item2, checkOrder.BranchId, orderFormCheck.Id, orderFile.ProductId, checkInventory.Id, cancellationToken))
                        throw new CaptiveException($"Account No: {checkOrder.AccountNo} has conflicted series");

                    if (_checkValidationService.HitEndingSeries(checkInventory, series.Item1, series.Item2))
                    {
                        if (!checkInventory.isRepeating)
                            throw new CaptiveException($"Account No: {checkOrder.AccountNo} hit the ending series!");

                        startingSeriesNumber = 1;
                        endingSeriesNumber = (startingSeriesNumber + orderFormCheck.Quantity) - 1;
                        series = _stringService.ConvertToSeries(checkInventory!.SeriesPatern, checkInventory.NumberOfPadding, startingSeriesNumber, endingSeriesNumber);
                        warningMsg = _checkValidationService.HitWarningSeries(checkInventory, series.Item1, series.Item2);
                    }

                    await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
                    {
                        Id = Guid.Empty,
                        ProductId = orderFile.ProductId,
                        CheckOrderId = checkOrder.Id,
                        StartingSeries = series.Item1,
                        EndingSeries = series.Item2,
                        CheckInventoryId = checkInventory.Id,
                        Quantity = orderFormCheck.Quantity,
                        BranchId = checkOrder.BranchId,
                        StartingNumber = startingSeriesNumber,
                        EndingNumber = endingSeriesNumber,
                        AccountNumber = checkOrder.AccountNo,
                        FormCheckId = orderFormCheck.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    }, cancellationToken);

                    startingSeriesNumber = endingSeriesNumber + 1;
                    endingSeriesNumber = (startingSeriesNumber + orderFormCheck.Quantity) - 1;
                    checkInventory.CurrentSeries = startingSeriesNumber;
                }
            }

            return logDto;
        }

        private CheckInventoryDetail? GetLastCheckDetail(Captive.Data.Models.CheckInventory checkInventory, CheckOrders checkOrder)
        {
            var mapping = new CheckInventoryMappingData(
                checkInventory.Mappings.Where(m => m.BranchId.HasValue).Select(m => m.BranchId!.Value),
                checkInventory.Mappings.Where(m => m.ProductId.HasValue).Select(m => m.ProductId!.Value),
                checkInventory.Mappings.Where(m => m.FormCheckType != null).Select(m => m.FormCheckType!)
            );

            var localQuery = _readUow.CheckInventoryDetails.GetAllLocal();
            var dbQuery = _readUow.CheckInventoryDetails.GetAll();

            localQuery = ApplyFilters(localQuery, mapping, checkOrder);
            dbQuery = ApplyFilters(dbQuery, mapping, checkOrder);

            var lastLocal = localQuery
                .Where(x => x.CheckInventoryId == checkInventory.Id)
                .OrderByDescending(x => x.EndingNumber)
                .FirstOrDefault();

            var lastDb = dbQuery
                .Where(x => x.CheckInventoryId == checkInventory.Id)
                .OrderByDescending(x => x.EndingNumber)
                .FirstOrDefault();

            if (lastLocal == null) return lastDb;
            if (lastDb == null) return lastLocal;

            return lastLocal.EndingNumber > lastDb.EndingNumber ? lastLocal : lastDb;
        }

        private IQueryable<CheckInventoryDetail> ApplyFilters(IQueryable<CheckInventoryDetail> query, CheckInventoryMappingData mapping, CheckOrders checkOrder)
        {
            if (mapping.BranchIds.Any())
                query = query.Where(x => x.BranchId == checkOrder.BranchId);

            if (mapping.ProductIds.Any())
                query = query.Where(x => x.ProductId == checkOrder.ProductId);

            if (mapping.FormCheckType.Any())
                query = query.Where(x => x.FormCheckId == checkOrder.FormCheckId);

            return query;
        }
    }
}
