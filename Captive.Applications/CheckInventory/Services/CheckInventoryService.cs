using Captive.Applications.CheckValidation.Services;
using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using Captive.Model.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Captive.Applications.CheckInventory.Services
{
    public interface ICheckInventoryService
    {
        Task<LogDto> ApplyCheckInventory(OrderFile orderFile, CancellationToken cancellationToken);
        Task<List<string>> GetInventoryWarnings(IEnumerable<OrderFile> orderFiles, CancellationToken cancellationToken);
    }

    public class CheckInventoryService : ICheckInventoryService
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckValidationService _checkValidationService;
        private readonly IStringService _stringService;
        private readonly IOrderFileNotifier _orderFileNotifier;

        public CheckInventoryService(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, ICheckValidationService checkValidationService, IStringService stringService, IOrderFileNotifier orderFileNotifier)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkValidationService = checkValidationService;
            _stringService = stringService;
            _orderFileNotifier = orderFileNotifier;
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

            int total = checkOrders.Length;
            for (int idx = 0; idx < checkOrders.Length; idx++)
            {
                var checkOrder = checkOrders[idx];
                await _orderFileNotifier.NotifyOrderFileProgress(
                    orderFile.BatchFileId, orderFile.Id,
                    $"Applying inventory ({idx + 1} of {total})",
                    cancellationToken);
                var orderFormCheck = await _readUow.FormChecks.GetAll().AsNoTracking().FirstAsync(x => x.Id == checkOrder.FormCheckId, cancellationToken);

                if (!string.IsNullOrEmpty(checkOrder.PreStartingSeries) && !string.IsNullOrEmpty(checkOrder.PreEndingSeries))
                {
                    await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
                    {
                        Id = Guid.Empty,
                        ProductId = orderFile.ProductId,
                        CheckOrderId = checkOrder.Id,
                        StartingSeries = checkOrder.PreStartingSeries,
                        EndingSeries = checkOrder.PreEndingSeries,
                        CheckInventoryId = null,
                        Quantity = orderFormCheck!.Quantity,
                        BranchId = checkOrder.BranchId,
                        AccountNumber = checkOrder.AccountNo,
                        FormCheckId = orderFormCheck.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    }, cancellationToken);

                    continue;
                }

                // Only the starting series was provided (e.g. custom order file): generate the
                // series from it instead of the check inventory.
                if (!string.IsNullOrWhiteSpace(checkOrder.PreStartingSeries))
                {
                    await ApplyManualStartingSeries(orderFile, checkOrder, orderFormCheck, cancellationToken);
                    continue;
                }

                var checkInventory = await _checkValidationService.GetCheckInventoryDirect(
                   bankId,
                   checkOrder.BranchId,
                   checkOrder.ProductId,
                   orderFormCheck.FormCheckType,
                   checkOrder.AccountNo,
                   cancellationToken);

                var startingSeriesNumber = checkInventory.CurrentSeries + 1;

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

                    checkInventory.CurrentSeries = endingSeriesNumber;
                    startingSeriesNumber = endingSeriesNumber + 1;
                    endingSeriesNumber = (startingSeriesNumber + orderFormCheck.Quantity) - 1;

                    _writeUow.CheckInventory.Update(checkInventory);
                }
            }

            return logDto;
        }

        /// <summary>
        /// Generates the series of a check order from its manually provided starting series.
        /// One detail is created per booklet (order quantity), each covering the form check quantity,
        /// so the overall ending series = starting series + (order quantity x form check quantity) - 1.
        /// </summary>
        private async Task ApplyManualStartingSeries(OrderFile orderFile, CheckOrders checkOrder, Data.Models.FormChecks orderFormCheck, CancellationToken cancellationToken)
        {
            var startingSeries = checkOrder.PreStartingSeries!.Trim();
            var match = Regex.Match(startingSeries, @"^(.*?)(\d+)$");

            if (!match.Success || match.Groups[2].Value.Length > 18)
                throw new CaptiveException($"Account No: {checkOrder.AccountNo} has an invalid starting series '{startingSeries}'. It must end with a number (e.g. A0000001).");

            if (orderFormCheck.Quantity <= 0)
                throw new CaptiveException($"Account No: {checkOrder.AccountNo} - form check {orderFormCheck.CheckType}/{orderFormCheck.FormType} has no check quantity configured.");

            var prefix = match.Groups[1].Value;
            var numberOfPadding = match.Groups[2].Value.Length;
            var startingNumber = long.Parse(match.Groups[2].Value);
            var lastNumber = startingNumber + ((long)checkOrder.Quantity * orderFormCheck.Quantity) - 1;

            if (lastNumber.ToString().Length > numberOfPadding)
                throw new CaptiveException($"Account No: {checkOrder.AccountNo} - the generated ending series exceeds {numberOfPadding} digits.");

            for (int i = 0; i < checkOrder.Quantity; i++)
            {
                var bookletStart = startingNumber + ((long)i * orderFormCheck.Quantity);
                var bookletEnd = bookletStart + orderFormCheck.Quantity - 1;
                var series = _stringService.ConvertToSeries(prefix, numberOfPadding, bookletStart, bookletEnd);

                await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
                {
                    Id = Guid.Empty,
                    ProductId = orderFile.ProductId,
                    CheckOrderId = checkOrder.Id,
                    StartingSeries = series.Item1,
                    EndingSeries = series.Item2,
                    CheckInventoryId = null,
                    Quantity = orderFormCheck.Quantity,
                    BranchId = checkOrder.BranchId,
                    StartingNumber = bookletStart,
                    EndingNumber = bookletEnd,
                    AccountNumber = checkOrder.AccountNo,
                    FormCheckId = orderFormCheck.Id,
                    CreatedDateTime = DateTime.UtcNow,
                }, cancellationToken);
            }

            // Keep the generated ending series on the check order (tracked entity, saved with the pipeline).
            checkOrder.PreStartingSeries = startingSeries;
            checkOrder.PreEndingSeries = _stringService.ConvertToSeries(prefix, numberOfPadding, startingNumber, lastNumber).Item2;
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

        // Read-only scan — no writes to DB. Returns warning messages for any order file
        // whose projected series would hit the WarningSeries threshold.
        public async Task<List<string>> GetInventoryWarnings(IEnumerable<OrderFile> orderFiles, CancellationToken cancellationToken)
        {
            var warnings = new List<string>();

            foreach (var orderFile in orderFiles)
            {
                var bankId = orderFile.BatchFile!.BankInfoId;

                var checkOrders = await _readUow.CheckOrders.GetAll()
                    .AsNoTracking()
                    .Where(x => x.OrderFileId == orderFile.Id)
                    .OrderBy(x => x.AccountNo)
                    .ToArrayAsync(cancellationToken);

                foreach (var checkOrder in checkOrders)
                {
                    if (!String.IsNullOrEmpty(checkOrder.PreStartingSeries) && !String.IsNullOrEmpty(checkOrder.PreStartingSeries))
                        continue;

                    var orderFormCheck = await _readUow.FormChecks.GetAll()
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == checkOrder.FormCheckId, cancellationToken);

                    var checkInventory = await _checkValidationService.GetCheckInventoryDirect(
                        bankId,
                        checkOrder.BranchId,
                        checkOrder.ProductId,
                        orderFormCheck.FormCheckType,
                        checkOrder.AccountNo,
                        cancellationToken);

                    if (checkInventory == null) continue;

                    var mapping = new CheckInventoryMappingData(
                        checkInventory.Mappings.Where(m => m.BranchId.HasValue).Select(m => m.BranchId!.Value),
                        checkInventory.Mappings.Where(m => m.ProductId.HasValue).Select(m => m.ProductId!.Value),
                        checkInventory.Mappings.Where(m => m.FormCheckType != null).Select(m => m.FormCheckType!)
                    );

                    var dbQuery = _readUow.CheckInventoryDetails.GetAll();
                    dbQuery = ApplyFilters(dbQuery, mapping, checkOrder);

                    var lastDetail = dbQuery
                        .Where(x => x.CheckInventoryId == checkInventory.Id)
                        .OrderByDescending(x => x.EndingNumber)
                        .FirstOrDefault();

                    var startingSeriesNumber = checkInventory.StartingSeries + 1;
                    if (lastDetail != null)
                        startingSeriesNumber = lastDetail.EndingNumber + 1;

                    var endingSeriesNumber = (startingSeriesNumber + orderFormCheck.Quantity) - 1;

                    var series = _stringService.ConvertToSeries(
                        checkInventory.SeriesPatern,
                        checkInventory.NumberOfPadding,
                        startingSeriesNumber,
                        endingSeriesNumber);

                    var warningMsg = _checkValidationService.HitWarningSeries(checkInventory, series.Item1, series.Item2);
                    if (!string.IsNullOrEmpty(warningMsg))
                    {
                        var detail = $"Account {checkOrder.AccountNo}: {warningMsg}";
                        if (!warnings.Contains(detail))
                            warnings.Add(detail);
                    }
                }
            }

            return warnings;
        }
    }
}
