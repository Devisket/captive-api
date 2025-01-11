using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Orderfiles.Command.DeleteOrderFile
{
    public class DeleteOrderFileCommandHandler : IRequestHandler<DeleteOrderFileCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public DeleteOrderFileCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(DeleteOrderFileCommand request, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null) 
            {
                throw new SystemException($"Order file ID: {request.OrderFileId} doesn't exist");
            }

            _writeUow.OrderFiles.Delete(orderFile);

            return Unit.Value;
        }
    }
}
