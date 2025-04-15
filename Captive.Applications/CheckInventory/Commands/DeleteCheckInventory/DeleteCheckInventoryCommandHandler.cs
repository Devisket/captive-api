using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Commands.DeleteCheckInventory
{
    public class DeleteCheckInventoryCommandHandler : IRequestHandler<DeleteCheckInventoryCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public DeleteCheckInventoryCommandHandler( IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(DeleteCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id);

            if (checkInventory == null) {
                throw new Exception($"Check Inventory ID: {request.Id} doesn't exist.");
            }

            _writeUow.CheckInventory.Delete(checkInventory);

            return Unit.Value;
        }
    }
}
