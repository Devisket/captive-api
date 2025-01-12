using Captive.Applications.FormsChecks.Services;
using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventoryDetails
{
    public class ApplyCheckInventoryDetailsCommandHandler : IRequestHandler<ApplyCheckInventoryDetailsCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly IFormsChecksService _formsChecksService;
        private readonly IStringService _stringService;

        public ApplyCheckInventoryDetailsCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow, IFormsChecksService formsChecksService, IStringService stringService) 
        {        
            _writeUow = writeUow;
            _readUow = readUow;
            _formsChecksService = formsChecksService;
            _stringService = stringService;
        }

        /**
         * - Get the check validation that was used by the order file
         * - Check if there is existing Check Inventory out of the check validation ID
         * - Apply the check inventory into the check order
         *      - if the check order dto doesn't have any starting/ending series, apply the series pattern from the check inventory. Other           wise validate the predefine series before applying it
         * - Tag the check inventory with corresponding branch, formcheck and checkOrder 
         *      
         **/

        public async Task<Unit> Handle(ApplyCheckInventoryDetailsCommand request, CancellationToken cancellationToken)
        {
            //var orderFile = await _readUow.OrderFiles.GetAll()
            //    .Include(x=> x.Product)
            //        .ThenInclude(x => x.ProductConfiguration)
            //    .Include(x => x.Product)
            //        .ThenInclude(x => x.ProductConfiguration)
            //        .ThenInclude(x => x.CheckValidation)
            //    .Include(x => x.CheckOrders)
            //    .FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            //if (orderFile == null)
            //{
            //    throw new Exception($"Order File ID: {request.OrderFileId} doesn't exist.");
            //}
            
            //var configuration = orderFile.Product.ProductConfiguration;

            //if (configuration == null) {

            //    throw new Exception($"Product Configuration is null.");
            //}

            //var checkInventory = await _readUow.CheckInventory.GetAll().Include(x => x.CheckValidation).FirstOrDefaultAsync(x => x.CheckValidationId == configuration.CheckValidationId);

            //if (checkInventory == null) {

            //    throw new Exception($"There is no check inventory record for the check validation id: {configuration.CheckValidationId}");
            //}
            
            //foreach(var checkOrder in orderFile.FloatingCheckOrders)
            //{
            //    var formCheck = await _formsChecksService.GetCheckOrderFormCheck(orderFile.ProductId, checkOrder.FormType, checkOrder.CheckType, cancellationToken);

            //    if (formCheck == null) 
            //    {
            //        throw new Exception("Form Check doesn't exist.");
            //    }


            //    var tag = configuration.CheckValidation.ValidationType == Data.Enums.ValidationType.Mix ? await GetTag(checkOrder, configuration.CheckValidation) : null;
                

            //    var pattern = checkInventory.SeriesPatern;

            //    var checkQuantity = formCheck?.Quantity ?? 0;

            //    for (int i = 0; i < checkOrder.Quantity; i++) 
            //    {
            //        var lastCheckDetailQuery = _readUow.CheckInventoryDetails.GetAllLocal().Where(x => x.CheckInventoryId == checkInventory.Id);

            //        if (tag != null)
            //            lastCheckDetailQuery.Where(x => x.TagId == tag.Id);

            //        var lastCheckDetailRecord = lastCheckDetailQuery.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();

            //        var tuple = _stringService.GetNextSeries(pattern, lastCheckDetailRecord?.EndingSeries, checkQuantity);

            //        var startingSeriesNo = !string.IsNullOrEmpty(checkOrder.PreStartingSeries) ? checkOrder.PreStartingSeries : tuple.Item1;

            //        var endingSeries = !string.IsNullOrEmpty(checkOrder.PreEndingSeries) ? checkOrder.PreEndingSeries : tuple.Item2;

            //        await _writeUow.CheckInventoryDetails.AddAsync(new CheckInventoryDetail
            //        {
            //            Id = Guid.Empty,
            //            ProductId = orderFile.ProductId,
            //            CheckOrderId = checkOrder.Id,
            //            StartingSeries = startingSeriesNo,
            //            EndingSeries = endingSeries,
            //            CheckInventoryId = checkInventory.Id,
            //            Quantity = checkQuantity,
            //            BranchId = checkOrder.BranchId,
            //            AccountNumber = checkOrder.AccountNo,
            //            FormCheckId = formCheck.Id,
            //            TagId = tag?.Id,
            //            CreatedDateTime = DateTime.UtcNow,
            //        }, cancellationToken);
            //    }
            //}          
            return Unit.Value;
        }

        private async Task<Tag?> GetTag(CheckOrders checkOrder, Captive.Data.Models.CheckValidation checkValidation)
        {
            var tagMappingQuery = _readUow.TagsMapping.GetAll().Include(x => x.Tag).Where(x => x.Tag.CheckValidationId == checkValidation.Id);

            if (checkValidation.ValidateByBranch)
            {
                tagMappingQuery.Where(x => x.BranchId == checkOrder.BranchId);
            }

            if (checkValidation.ValidateByProduct)
            {
                tagMappingQuery.Where(x => x.ProductId == checkOrder.ProductId);
            }

            if (checkValidation.ValidateByFormCheck)
            {
                tagMappingQuery.Where(x => x.FormCheckId == checkOrder.FormCheckId);
            }
            var tagMapping = await tagMappingQuery.FirstOrDefaultAsync();

            return tagMapping?.Tag;
        }
    }
}
