using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Command.UpdateCheckOrderBarcode
{
    internal class UpdateCheckOrderBarcodeCommandHandler : IRequestHandler<UpdateCheckOrderBarcodeCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public UpdateCheckOrderBarcodeCommandHandler(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(UpdateCheckOrderBarcodeCommand request, CancellationToken cancellationToken)
        {
            var checkOrders = await _readUow.CheckOrders
                .GetAll()
                .Include(x => x.OrderFile)
                .Where(x => x.OrderFile.BatchFileId == request.BatchId)
                .ToListAsync(cancellationToken);

            if (!checkOrders.Any())
            {
                return Unit.Value;
            }

            foreach (var checkOrder in checkOrders.Where(x => request.CheckordersToUpdate.Any(z => z.CheckOrderId == x.Id)))
            {
                var updateRequest = request.CheckordersToUpdate
                    .FirstOrDefault(x => x.CheckOrderId == checkOrder.Id);

                if (updateRequest != null)
                {
                    checkOrder.BarCodeValue = updateRequest.BarcodeValue;
                    _writeUow.CheckOrders.Update(checkOrder);
                }
            }

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
