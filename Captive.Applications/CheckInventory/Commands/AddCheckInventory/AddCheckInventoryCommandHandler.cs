using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;

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
            var nextSeries = 0;
            var formCheck = _readUow.FormChecks.GetAll().Where(x => x.Id == request.FormCheckId).FirstOrDefault();


            if (formCheck == null)
            {
                throw new Exception("Form check doesn't exist");
            }

            var existingCheckInventory = _readUow.CheckInventory.GetAll().Any(x => x.FormCheckId == request.FormCheckId);

            if (existingCheckInventory)
            {
                var endingCheck = _readUow.CheckInventory.GetAll().Where(x => x.FormCheckId == request.FormCheckId).OrderBy(x => x.Id).LastOrDefault();

                if(endingCheck != null)
                {
                    if(endingCheck.EndSeries != null)
                    {
                        nextSeries = int.Parse(endingCheck.EndSeries) + 1;
                    }
                }
            }
            
            if(nextSeries == 0)
            {
                nextSeries += 1;
            }

          for(var i = 0; i < request.Quantity; i++)
            {
                var endingSeries = nextSeries + formCheck.Quantity;

                await _writeUow.CheckInventory.Add(new Data.Models.CheckInventory
                {
                    StarSeries = String.Format("{0:D8}", nextSeries),
                    EndSeries = String.Format("{0:D8}", endingSeries),
                    FormCheckId = request.FormCheckId,
                    Quantity = formCheck.Quantity
                });

                nextSeries = endingSeries + 1;
            }

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
