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

            var productConfiguration = await _readUow.ProductConfigurations.GetAll()
                .AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == orderFile.ProductId, cancellationToken);

            var checkOrders = _readUow.CheckOrders.GetAllLocal().Where(x => x.OrderFileId == orderFile.Id).ToArray();

            foreach (var checkOrder in checkOrders)
            {
                var formCheck = await _readUow.FormChecks.GetAll().AsNoTracking().FirstAsync(x => x.Id == checkOrder.FormCheckId, cancellationToken);

                var bankId = orderFile.BatchFile!.BankInfoId;

                var tag = await _checkValidationService.GetTag(bankId, checkOrder.BranchId, checkOrder.ProductId, formCheck.FormCheckType, cancellationToken);

                var checkInventory = await _checkValidationService.GetCheckInventory(tag, checkOrder.BranchId, checkOrder.ProductId, formCheck.FormCheckType, cancellationToken);

                if (!string.IsNullOrEmpty(checkOrder.PreStartingSeries) && !string.IsNullOrEmpty(checkOrder.PreEndingSeries))
                { 
                    if (formCheck.HasBranchCodeInSeries)
                    {
                        //checkOrder.PreStartingSeries = String.Concat()
                    }

                    await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
                    {
                        Id = Guid.Empty,
                        ProductId = orderFile.ProductId,
                        CheckOrderId = checkOrder.Id,
                        StartingSeries = checkOrder.PreStartingSeries,
                        EndingSeries = checkOrder.PreEndingSeries,
                        CheckInventoryId = checkInventory!.Id,
                        Quantity = formCheck!.Quantity,
                        BranchId = checkOrder.BranchId,
                        AccountNumber = checkOrder.AccountNo,
                        FormCheckId = formCheck.Id,
                        TagId = tag?.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    }, cancellationToken);

                    continue;
                }

                var startingSeriesNumber = checkInventory.StartingSeries;

                var lastCheck = GetLastCheckDetail(tag, checkOrder, checkInventory.Id);

                if (lastCheck != null)
                    startingSeriesNumber = lastCheck.EndingNumber + 1;

                var endingSeriesNumber = (startingSeriesNumber + formCheck.Quantity) - 1;

                for (int i = 1; i <= checkOrder.Quantity; i++)
                {
                    var series = _stringService.ConvertToSeries(checkInventory!.SeriesPatern, checkInventory.NumberOfPadding, startingSeriesNumber, endingSeriesNumber);

                    var warningMsg = _checkValidationService.HitWarningSeries(checkInventory, series.Item1, series.Item2);

                    if(!string.IsNullOrEmpty(warningMsg))
                    {
                        logDto.LogType = Model.Enums.LogType.Warning;
                        logDto.LogMessage = warningMsg;
                    }

                    if (await _checkValidationService.HasConflictedSeries(series.Item1, series.Item2, checkOrder.BranchId, formCheck.Id, orderFile.ProductId, tag.Id, cancellationToken))
                        throw new CaptiveException($"Account No: {checkOrder.AccountNo} has conflicted series");

                    if (_checkValidationService.HitEndingSeries(checkInventory, series.Item1, series.Item2))
                    {
                        if(!checkInventory.isRepeating)
                            throw new CaptiveException($"Account No: {checkOrder.AccountNo} hit the ending series!");

                        startingSeriesNumber = 1;
                        endingSeriesNumber = (startingSeriesNumber + formCheck.Quantity) - 1;
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
                        Quantity = formCheck.Quantity,
                        BranchId = checkOrder.BranchId,
                        StartingNumber = startingSeriesNumber,
                        EndingNumber = endingSeriesNumber,
                        AccountNumber = checkOrder.AccountNo,
                        FormCheckId = formCheck.Id,
                        TagId = tag?.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    }, cancellationToken);

                    startingSeriesNumber = endingSeriesNumber + 1;

                    endingSeriesNumber = (startingSeriesNumber + formCheck.Quantity) - 1;

                    checkInventory.CurrentSeries = startingSeriesNumber;
                }
            }

            return logDto;
        }


        private CheckInventoryDetail? GetLastCheckDetail(Tag tag, CheckOrders checkOrder, Guid checkInventoryId)
        {
            // Get both local and database records
            var localQuery = _readUow.CheckInventoryDetails.GetAllLocal();
            var dbQuery = _readUow.CheckInventoryDetails.GetAll();

            // Apply filters to both queries
            localQuery = ApplyFilters(localQuery, tag, checkOrder);
            dbQuery = ApplyFilters(dbQuery, tag, checkOrder);

            // Get the last record from both sources
            var lastLocal = localQuery
                .Where(x => x.CheckInventoryId == checkInventoryId)
                .OrderByDescending(x => x.EndingNumber)
                .FirstOrDefault();

            var lastDb = dbQuery
                .Where(x => x.CheckInventoryId == checkInventoryId)
                .OrderByDescending(x => x.EndingNumber)
                .FirstOrDefault();

            // Return the record with the highest EndingNumber
            if (lastLocal == null) return lastDb;
            if (lastDb == null) return lastLocal;
            
            return lastLocal.EndingNumber > lastDb.EndingNumber ? lastLocal : lastDb;
        }

        private IQueryable<CheckInventoryDetail> ApplyFilters(IQueryable<CheckInventoryDetail> query, Tag tag, CheckOrders checkOrder)
        {
            // Branch + Product + FormCheck combination
            if (tag.SearchByBranch && tag.SearchByProduct && tag.SearchByFormCheck)
            {
                return query.Where(x => 
                    x.BranchId == checkOrder.BranchId && 
                    x.ProductId == checkOrder.ProductId && 
                    x.FormCheckId == checkOrder.FormCheckId);
            }
            // Branch + Product combination
            else if (tag.SearchByBranch && tag.SearchByProduct)
            {
                return query.Where(x => 
                    x.BranchId == checkOrder.BranchId && 
                    x.ProductId == checkOrder.ProductId);
            }
            // Branch + FormCheck combination
            else if (tag.SearchByBranch && tag.SearchByFormCheck)
            {
                return query.Where(x => 
                    x.BranchId == checkOrder.BranchId && 
                    x.FormCheckId == checkOrder.FormCheckId);
            }
            // Product + FormCheck combination
            else if (tag.SearchByProduct && tag.SearchByFormCheck)
            {
                return query.Where(x => 
                    x.ProductId == checkOrder.ProductId && 
                    x.FormCheckId == checkOrder.FormCheckId);
            }
            // Single criteria
            else
            {
                if (tag.SearchByBranch)
                    query = query.Where(x => x.BranchId == checkOrder.BranchId);

                if (tag.SearchByProduct)
                    query = query.Where(x => x.ProductId == checkOrder.ProductId);

                if (tag.SearchByFormCheck)
                    query = query.Where(x => x.FormCheckId == checkOrder.FormCheckId);
            }

            return query;
        }
    }
}
