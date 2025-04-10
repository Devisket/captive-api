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
            if (request.Id.HasValue)
            {
                var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(cancellationToken);

                if (checkInventory == null) {
                    throw new Exception($"Check Inventory ID: {request.Id} doesn't exist");
                }

                checkInventory.WarningSeries = request.WarningSeries;
                checkInventory.SeriesPatern = request.SeriesPattern;
                checkInventory.StartingSeries = request.StartingSeries;
                checkInventory.EndingSeries = request.EndingSeries;
                checkInventory.NumberOfPadding = request.NumberOfPadding;
                checkInventory.isRepeating = request.IsRepeating;

                _writeUow.CheckInventory.Update(checkInventory);
            }
            else
            {
                var newCheckInventory = new Captive.Data.Models.CheckInventory()
                {
                    Id = Guid.NewGuid(),
                    TagId = request.TagId,
                    SeriesPatern = request.SeriesPattern,
                    WarningSeries = request.WarningSeries,
                    StartingSeries = request.StartingSeries,
                    EndingSeries = request.EndingSeries,
                    NumberOfPadding = request.NumberOfPadding,
                    isRepeating = request.IsRepeating
                };

                await _writeUow.CheckInventory.AddAsync(newCheckInventory,cancellationToken);
            }
            
            return Unit.Value;
        }
    }
}
