using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventoryDetails
{
    public class AddCheckInventoryDetailsCommandHandler : IRequestHandler<AddCheckInventoryDetailsCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public AddCheckInventoryDetailsCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {        
            _writeUow = writeUow;
            _readUow = readUow;
        }
        public Task<Unit> Handle(AddCheckInventoryDetailsCommand request, CancellationToken cancellationToken)
        {

            //var orderFile = _readUow.OrderFiles.GetAll().AsNoTracking().Include(x => x.Product).ThenInclude(x => x.)
            throw new NotImplementedException();
        }
    }
}
