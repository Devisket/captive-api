using Captive.Applications.FormsChecks.Services;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventoryDetails
{
    public class ApplyCheckInventoryDetailsCommandHandler : IRequestHandler<ApplyCheckInventoryDetailsCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly IFormsChecksService _formsChecksService;

        public ApplyCheckInventoryDetailsCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow, IFormsChecksService formsChecksService) 
        {        
            _writeUow = writeUow;
            _readUow = readUow;
            _formsChecksService = formsChecksService;
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
            var orderFile = await _readUow.OrderFiles.GetAll().Include(x=> x.Product).ThenInclude(x => x.ProductConfiguration).FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null)
            {
                throw new Exception($"Order File ID: {request.OrderFileId} doesn't exist.");
            }
            
            var configuration = orderFile.Product.ProductConfiguration;

            if (configuration == null) {

                throw new Exception($"Product Configuration is null.");
            }

            var checkInventory = await _readUow.CheckInventory.GetAll().Include(x => x.CheckValidation).FirstOrDefaultAsync(x => x.CheckValidationId == configuration.CheckValidationId);

            if (checkInventory == null) {

                throw new Exception($"There is no check inventory record for the check validation id: {configuration.CheckValidationId}");
            }
            
            foreach(var checkOrder in request.CheckOrders)
            {
               
                var formCheck = await _formsChecksService.GetCheckOrderFormCheck(checkOrder, cancellationToken);

                if (formCheck == null) 
                { 
                    
                }

                if (!String.IsNullOrEmpty(checkOrder.StartingSeries))
                {
                    if (String.IsNullOrEmpty(checkOrder.EndingSeries))
                        throw new Exception("Starting series is defined but the ending series is not.");

                    await ValidateCheckSeries(checkInventory, checkOrder.StartingSeries, checkOrder.EndingSeries);
                }
                else
                {
                    var pattern = checkInventory.SeriesPatern;

                    var checkQuantity = formCheck?.Quantity ?? 1 * checkOrder.Quantity;

                    var lastCheckDetailRecord = _readUow.CheckInventoryDetails.GetAll().OrderByDescending(x => x.Sequence).FirstOrDefault();

                    var startingSeriesNo = lastCheckDetailRecord != null ? lastCheckDetailRecord.EndingSeries : ConvertPatternIntoSeries(checkInventory.SeriesPatern, 1);

                    var startingSeries = startingSeriesNo;

                    var endingSeries = ConvertPatternIntoSeries(checkInventory.SeriesPatern, checkQuantity);
                }
            }          
            return Unit.Value;
        }

             
        private string ConvertPatternIntoSeries(string pattern, int number)
        {
            var paddingCount = pattern.Count(x => x == '0');
            var seriesValue = number.ToString().PadLeft(paddingCount, '0');

            return string.Concat(pattern.Replace("0",string.Empty), seriesValue);            
        }

        /*
         * Validate Check Inventory
         * 1. Build check Inventory Details query out of checkValidation type
         */
        private async Task<bool> ValidateCheckSeries (Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries)
        {
            var checkValidation = checkInventory.CheckValidation;

            //var checkInventoryQuery = _readUow.CheckInventoryDetails.GetAll().Where(x => x.CheckInventoryId == CheckInventoryId);


            //var checkInventoryDetails = await _readUow.CheckInventoryDetails.GetAll().AsNoTracking().AnyAsync(x => x.CheckInventoryId == CheckInventoryId);

            return false;
        }

    }
}
