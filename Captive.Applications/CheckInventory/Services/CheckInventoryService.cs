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
                var formCheck = await _readUow.FormChecks.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == checkOrder.FormCheckId, cancellationToken);

                var bankId = orderFile.BatchFile!.BankInfoId;

                var tag = _checkValidationService.GetTag(bankId, checkOrder.BranchId, checkOrder.FormCheckId!.Value, checkOrder.ProductId);

                var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(x => x.TagId == tag.Id && x.IsActive, cancellationToken);

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
                        Quantity = formCheck!.Quantity,
                        BranchId = checkOrder.BranchId,
                        AccountNumber = checkOrder.AccountNo,
                        FormCheckId = formCheck.Id,
                        TagId = tag?.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    }, cancellationToken);

                    continue;
                }

                var startingSeriesNumber = 1;

                var lastCheck = _readUow.CheckInventoryDetails.GetAllLocal().OrderByDescending(x =>x.EndingNumber).FirstOrDefault(x => x.CheckInventoryId == checkInventory.Id);

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
                        throw new Exception($"Account No: ${checkOrder.AccountNo} has conflicted series");

                    if (_checkValidationService.HitEndingSeries(checkInventory, series.Item1, series.Item2))
                        throw new Exception($"Account No: ${checkOrder.AccountNo} hit the ending series!");

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
    }
}
