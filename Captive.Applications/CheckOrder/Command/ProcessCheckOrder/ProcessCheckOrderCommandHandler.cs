using Captive.Applications.CheckInventory.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckOrder.Command.ProcessCheckOrder
{
    public class ProcessCheckOrderCommandHandler : IRequestHandler<ProcessCheckOrderCommand, ProcessCheckOrderCommandResponse>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckOrderService _checkOrderService;
        private readonly ICheckInventoryService _checkInventoryService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IOrderFileService _orderFileService;
        private readonly IProducer<DbfGenerateMessage> _producer;

        public ProcessCheckOrderCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, ICheckOrderService checkOrderService, ICheckInventoryService checkInventoryService, IReportGenerator reportGenerator, IOrderFileService orderFileService, IProducer<DbfGenerateMessage> producer)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkOrderService = checkOrderService;
            _checkInventoryService = checkInventoryService;
            _reportGenerator = reportGenerator;
            _orderFileService = orderFileService;
            _producer = producer;
        }

        public async Task<ProcessCheckOrderCommandResponse> Handle(ProcessCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var returnObj = new ProcessCheckOrderCommandResponse { };
            var orderFile = await _readUow.OrderFiles
                .GetAll()
                .Include(x => x.BatchFile)
                .Include(x => x.FloatingCheckOrders)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null)
                throw new SystemException($"Order file ID {request.OrderFileId} doesn't exist");

            await _checkOrderService.CreateCheckOrder(orderFile, cancellationToken);
            var logDto = await _checkInventoryService.ApplyCheckInventory(orderFile, cancellationToken);

            if (!String.IsNullOrEmpty(logDto.LogMessage))
            {
                returnObj.LogMessage = logDto.LogMessage;
                returnObj.LogType = logDto.LogType;
            }

            await _orderFileService.UpdateOrderFileStatus(orderFile.Id, Data.Enums.OrderFilesStatus.Completed, cancellationToken);

            await _writeUow.Complete();

            await _reportGenerator.OnGenerateReport(orderFile.BatchFileId, cancellationToken);

            _producer.ProduceMessage(new DbfGenerateMessage
            {
                BatchId = orderFile.BatchFileId
            });

            return returnObj;
        }
    }
}
