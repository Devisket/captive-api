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

        public ApplyCheckInventoryDetailsCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {        
            _writeUow = writeUow;
            _readUow = readUow;
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

            var orderFile = await _readUow.OrderFiles.GetAll().Include(x=> x.Product).ThenInclude(x => x.ProductConfigurations).FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null)
            {
                throw new Exception($"Order File ID: {request.OrderFileId} doesn't exist.");
            }
            
            var configuration = orderFile.Product.ProductConfigurations;

            if (configuration == null) {

                throw new Exception($"Product Configuration is null.");
            }

            var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(x => x.CheckValidationId == configuration.CheckValidationId);

            if (checkInventory == null) {

                throw new Exception($"There is no check inventory record for the check validation id: {configuration.CheckValidationId}");
            }
            
            foreach(var checkOrder in request.CheckOrders)
            {
                if (!String.IsNullOrEmpty(checkOrder.StartingSeries))
                {
                    if (String.IsNullOrEmpty(checkOrder.EndingSeries))
                        throw new Exception("Starting series is defined but the ending series is not.");

                    await ValidateCheckSeries(configuration.CheckValidationId, checkOrder.StartingSeries, checkOrder.EndingSeries);
                }
                else
                {
                                      
                }
            }          
            return Unit.Value;
        }
        
        public async Task<bool> ValidateCheckSeries (Guid checkValidationId, string startingSeries, string endingSeries)
        {
            var checkValidation = await _readUow.CheckValidations.GetAll().AsNoTracking().Include(x => x.CheckInventory).FirstOrDefaultAsync(x => x.Id == checkValidationId);

            if (checkValidation == null)
            {
                throw new Exception($"Check Validation doesn't exist for ID: {checkValidationId}");
            }

            var checkInventoryDetails = await _readUow.CheckInventoryDetails.GetAll().AsNoTracking().AnyAsync(x => x.CheckInventoryId == checkValidation.CheckInventoryId);

            return false;
        }

    }
}
