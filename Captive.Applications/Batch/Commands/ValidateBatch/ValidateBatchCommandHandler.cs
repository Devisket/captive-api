using Captive.Applications.CheckOrder.Services;
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



        public ValidateBatchCommandHandler(ICheckOrderService checkOrderService, IWriteUnitOfWork writeUow, IReadUnitOfWork readUow) 
        {
            _checkOrderService = checkOrderService;
            _writeUow = writeUow;
            _readUow = readUow;
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

                if (!floatingChecks.Any(x => !x.IsValid))
                {
                    orderFile.IsValidated = true;
                    orderFile.Status = OrderFilesStatus.Valid;
                } else
                    orderFile.Status = OrderFilesStatus.Invalid;
                
                orderFile.PersonalQuantity = tupleObj.Item2;
                orderFile.CommercialQuantity = tupleObj.Item3;

                _writeUow.OrderFiles.Update(orderFile);
            }

            return logRecords;
        }
    }
}
