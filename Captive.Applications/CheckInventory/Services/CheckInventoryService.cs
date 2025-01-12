using Captive.Applications.CheckValidation.Services;
using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Services
{
    public interface ICheckInventoryService
    {
        Task ApplyCheckInventory(OrderFile orderFile, CancellationToken cancellationToken);
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

        public async Task ApplyCheckInventory(OrderFile orderFile, CancellationToken cancellationToken)
        {
            var productConfiguration = await _readUow.ProductConfigurations.GetAll()
                .Include(x => x.CheckValidation)
                .ThenInclude(x => x.Tags)
                .ThenInclude(x => x.Mapping)
                .AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == orderFile.ProductId, cancellationToken);

            var tags = productConfiguration.CheckValidation.Tags.ToArray();

            var checkOrders = _readUow.CheckOrders.GetAllLocal().Where(x => x.OrderFileId == orderFile.Id).ToArray();

            foreach (var checkOrder in checkOrders)
            {
                var formCheck = await _readUow.FormChecks.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == checkOrder.FormCheckId, cancellationToken);
                var tag = _checkValidationService.GetTag(tags, checkOrder.BranchId, checkOrder.FormCheckId.Value, checkOrder.ProductId);

                var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(x => x.TagId == tag.Id);

                if (!string.IsNullOrEmpty(checkOrder.PreStartingSeries) && !string.IsNullOrEmpty(checkOrder.PreEndingSeries))
                {
                    await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
                    {
                        Id = Guid.Empty,
                        ProductId = orderFile.ProductId,
                        CheckOrderId = checkOrder.Id,
                        StartingSeries = checkOrder.PreStartingSeries,
                        EndingSeries = checkOrder.PreEndingSeries,
                        CheckInventoryId = checkInventory.Id,
                        Quantity = formCheck.Quantity,
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
                    var series = _stringService.ConvertToSeries(checkInventory.SeriesPatern, checkInventory.NumberOfPadding, startingSeriesNumber, endingSeriesNumber);

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
                }
            }
        }
    }
}
