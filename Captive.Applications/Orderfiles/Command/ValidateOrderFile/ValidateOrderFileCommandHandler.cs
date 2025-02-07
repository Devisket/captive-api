using Captive.Applications.CheckOrder.Services;
using Captive.Data.Models;
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
            var returnObj = await _checkOrderService.ValidateCheckOrder(request.OrderFileId, cancellationToken);

            var floatingChecks = returnObj.Item1;

            _writeUow.FloatingCheckOrders.UpdateRange(floatingChecks);

            if(!floatingChecks.Any(x => !x.IsValid))
            {
                var orderFile = _readUow.OrderFiles.GetAll().First(x => x.Id == request.OrderFileId);

                orderFile.IsValidated = true;

                _writeUow.OrderFiles.Update(orderFile);
            }

            return new ValidateOrderFileCommandResponse
            {
                personalQuantity = returnObj.Item2,
                commercialQuantity = returnObj.Item3,
            };
        }
    }
}
