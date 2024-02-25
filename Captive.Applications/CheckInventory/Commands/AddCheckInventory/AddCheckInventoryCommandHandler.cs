using Captive.Data.Models;
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

            var formCheck = _readUow.FormChecks.GetAll().Where(x => x.Id == request.FormCheckId).FirstOrDefault();

            if (formCheck == null)
            {
                throw new Exception("Form check doesn't exist");
            }

            if (request.WithSeries)
                await AddCheckInventoryWithSeries(formCheck, request.Quantity, cancellationToken);
            else
                await AddNoCheckInventoryWithNoSeries(formCheck, request.Quantity, cancellationToken);

            return Unit.Value;
        }

        private async Task AddCheckInventoryWithSeries(FormChecks formCheck, int checkQuantity, CancellationToken cancellationToken)
        {
            var nextSeries = 0;

            var existingCheckInventory = _readUow.CheckInventory.GetAll()
                .AsNoTracking()
                .Any(x => x.FormChecks == formCheck);

            if (existingCheckInventory)
            {
                var endingCheck = _readUow.CheckInventory.GetAll()
                    .AsNoTracking()
                    .Where(x => x.FormChecks == formCheck)
                    .OrderBy(x => x.Id).LastOrDefault();

                if (endingCheck != null)
                {
                    if (endingCheck.EndSeries != null)
                    {
                        nextSeries = int.Parse(endingCheck.EndSeries) + 1;
                    }
                }
            }

            if (nextSeries == 0)
                nextSeries += 1;
            
            for (var i = 0; i < checkQuantity; i++)
            {
                var endingSeries = nextSeries + formCheck.Quantity;
                endingSeries -= 1;

                var checkInventory = new Data.Models.CheckInventory
                {
                    StarSeries = String.Format("{0:D8}", nextSeries),
                    EndSeries = String.Format("{0:D8}", endingSeries),
                    FormCheckId = formCheck.Id,
                    Quantity = formCheck.Quantity
                };

                await InsertCheckInventory(checkInventory, cancellationToken);
                nextSeries = endingSeries + 1;
            }

            await _writeUow.Complete();
        }

        private async Task AddNoCheckInventoryWithNoSeries(FormChecks formCheck, int checkQuantity, CancellationToken cancellationToken)
        {
            for (var i = 0; i < checkQuantity; i++)
            {
                var checkInventory = new Data.Models.CheckInventory
                {
                    FormCheckId = formCheck.Id,
                    Quantity = formCheck.Quantity
                };

                await InsertCheckInventory(checkInventory, cancellationToken);
            }

            await _writeUow.Complete();
        }

        private async Task InsertCheckInventory(Data.Models.CheckInventory checkInventory, CancellationToken cancellationToken)
        {
            await _writeUow.CheckInventory.AddAsync(checkInventory, cancellationToken);
        }
    }
}
