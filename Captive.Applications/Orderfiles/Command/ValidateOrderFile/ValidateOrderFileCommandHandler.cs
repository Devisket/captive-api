using Captive.Applications.CheckOrder.Services;
using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;

namespace Captive.Applications.Orderfiles.Command.ValidateOrderFile
{
    public class ValidateOrderFileCommandHandler : IRequestHandler<ValidateOrderFileCommand, ValidateOrderFileCommandResponse>
    {
        private readonly ICheckOrderService _checkOrderService;
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        public ValidateOrderFileCommandHandler(ICheckOrderService checkOrderService, IWriteUnitOfWork writeUow, IReadUnitOfWork readUow) 
        {
            _checkOrderService = checkOrderService;
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<ValidateOrderFileCommandResponse> Handle(ValidateOrderFileCommand request, CancellationToken cancellationToken)
        {
            var returnObj = new ValidateOrderFileCommandResponse { };

            var tupleObj = await _checkOrderService.ValidateCheckOrder(request.OrderFileId, cancellationToken);

            var logDto = tupleObj.Item4;

            if(!String.IsNullOrEmpty(logDto.LogMessage))
            {
                returnObj.LogType = logDto.LogType;
                returnObj.Message = logDto.LogMessage;
            }

            var orderFile = _readUow.OrderFiles.GetAll().First(x => x.Id == request.OrderFileId);

            var floatingChecks = tupleObj.Item1;

            _writeUow.FloatingCheckOrders.UpdateRange(floatingChecks);

            if (!floatingChecks.Any(x => !x.IsValid))
            {
                orderFile.IsValidated = true;
                orderFile.Status = OrderFilesStatus.Valid;
            }
            else
                orderFile.Status = OrderFilesStatus.Invalid;

            orderFile.PersonalQuantity = tupleObj.Item2;
            orderFile.CommercialQuantity = tupleObj.Item3;

            _writeUow.OrderFiles.Update(orderFile);

            return returnObj;
        }
    }
}
