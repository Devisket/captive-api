using Captive.Applications.Orderfiles.Services;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using MediatR;

namespace Captive.Applications.CheckOrder.Command.ProcessCheckOrder
{
    public class ProcessCheckOrderCommandHandler : IRequestHandler<ProcessCheckOrderCommand, ProcessCheckOrderCommandResponse>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IOrderFileService _orderFileService;
        private readonly IProducer<CheckOrderProcessMessage> _producer;

        public ProcessCheckOrderCommandHandler(
            IWriteUnitOfWork writeUow,
            IOrderFileService orderFileService,
            IProducer<CheckOrderProcessMessage> producer)
        {
            _writeUow = writeUow;
            _orderFileService = orderFileService;
            _producer = producer;
        }

        public async Task<ProcessCheckOrderCommandResponse> Handle(ProcessCheckOrderCommand request, CancellationToken cancellationToken)
        {
            await _orderFileService.UpdateOrderFileStatus(request.OrderFileId, Data.Enums.OrderFilesStatus.Processing, cancellationToken);
            await _writeUow.Complete(cancellationToken);

            _producer.ProduceMessage(new CheckOrderProcessMessage
            {
                OrderFileId = request.OrderFileId,
            });

            return new ProcessCheckOrderCommandResponse();
        }
    }
}
