using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Commands.SetCheckInventoryActive
{
    public class SetCheckInventoryActiveCommandHandler : IRequestHandler<SetCheckInventoryActiveCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public SetCheckInventoryActiveCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
            _writeUow = writeUow;   
        }

        public async Task<Unit> Handle(SetCheckInventoryActiveCommand request, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(x => x.Id == request.CheckInventoryId, cancellationToken);

            if (checkInventory == null) 
            {
                throw new Exception($"Check Inventory ID: {request.CheckInventoryId} doesn't exist");
            }

            checkInventory.IsActive = true;

            var otherActiveCheckInventory = await _readUow.CheckInventory.GetAll().Where(x => x.TagId == checkInventory.TagId  && x.IsActive).FirstOrDefaultAsync(cancellationToken);

            if (otherActiveCheckInventory != null) { 
                otherActiveCheckInventory.IsActive = false;
                _writeUow.CheckInventory.Update(otherActiveCheckInventory);
            }

            _writeUow.CheckInventory.Update(checkInventory);

            return Unit.Value;
        }
    }
}
