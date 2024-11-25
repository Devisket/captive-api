using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommandHandler : IRequestHandler<AddCheckInventoryCommand, Unit>
    {

        IReadUnitOfWork _readUow;
        IWriteUnitOfWork _writeUow;
        
        public AddCheckInventoryCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow) 
        { 
            _writeUow = writeUow;
            _readUow = readUow; 
        }

        public async Task<Unit> Handle(AddCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            var checkValidation = await _readUow.CheckValidations.GetAll().FirstOrDefaultAsync(x => x.Id == request.CheckValidationId);

            if(checkValidation == null)
            {
                throw new Exception($"Check Validation ID : {request.CheckValidationId} doesn't exist");
            }

            if (request.Id.HasValue)
            {
                var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(cancellationToken);

                if (checkInventory == null) {
                    throw new Exception($"Check Inventory ID: {request.Id} doesn't exist");
                }

                checkInventory.CheckValidationId = request.CheckValidationId;
                checkInventory.SeriesPatern = request.SeriesPattern;

                _writeUow.CheckInventory.Update(checkInventory);
            }
            else
            {
                var newCheckInventory = new Captive.Data.Models.CheckInventory()
                {
                    CheckValidationId = checkValidation.Id,
                    SeriesPatern = request.SeriesPattern,
                };

                await _writeUow.CheckInventory.AddAsync(newCheckInventory,cancellationToken);
            }
            
            return Unit.Value;
        }
    }
}
