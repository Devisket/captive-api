using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Commands.ValidateBatch
{
    public class ValidateBatchCommandHandler : IRequestHandler<ValidateBatchCommand, IEnumerable<LogDto>?>
    {
        private readonly ICheckOrderService _checkOrderService;
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IOrderFileService _orderFileService;

        public ValidateBatchCommandHandler(ICheckOrderService checkOrderService, IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, IOrderFileService orderFileService) 
        {
            _checkOrderService = checkOrderService;
            _writeUow = writeUow;
            _readUow = readUow;
            _orderFileService = orderFileService;
        }

        public async Task<IEnumerable<LogDto>?> Handle(ValidateBatchCommand request, CancellationToken cancellationToken)
        {
            var logRecords = new List<LogDto>();

            var batch = await _readUow.BatchFiles.GetAll().Include(x => x.OrderFiles).FirstOrDefaultAsync(x => x.Id == request.BatchId, cancellationToken);

            if (batch == null)
                throw new Exception($"BatchID {request.BatchId} doesn't exist.");

            if (batch.OrderFiles == null || !batch.OrderFiles.Any())
                return null;

            var orderFileIds = batch.OrderFiles!.Where(x => x.Status != OrderFilesStatus.Completed).Select(x => x.Id).ToArray();

            foreach(var orderFileId in orderFileIds)
            {

                var logRecord = new LogDto { };

                var tupleObj = await _checkOrderService.ValidateCheckOrder(orderFileId, cancellationToken);

                var logDto = tupleObj.Item4;

                if (!String.IsNullOrEmpty(logDto.LogMessage))
                {
                    logRecord.LogType = logDto.LogType;
                    logRecord.LogMessage = logDto.LogMessage;

                    logRecords.Add(logRecord);
                }

                var orderFile = _readUow.OrderFiles.GetAll().First(x => x.Id == orderFileId);

                var floatingChecks = tupleObj.Item1;

                _writeUow.FloatingCheckOrders.UpdateRange(floatingChecks);

                // Update quantities first
                orderFile.PersonalQuantity = tupleObj.Item2;
                orderFile.CommercialQuantity = tupleObj.Item3;
                _writeUow.OrderFiles.Update(orderFile);

                // Update status through the service to trigger SignalR notifications
                if (!floatingChecks.Any(x => !x.IsValid))
                {
                    orderFile.IsValidated = true;
                    await _orderFileService.UpdateOrderFileStatus(orderFileId, OrderFilesStatus.Valid, cancellationToken);
                }
                else
                {
                    await _orderFileService.UpdateOrderFileStatus(orderFileId, OrderFilesStatus.Invalid, cancellationToken);
                }
            }

            return logRecords;
        }
    }
}
