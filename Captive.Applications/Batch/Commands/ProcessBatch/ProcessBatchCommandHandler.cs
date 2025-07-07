using Captive.Applications.CheckInventory.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Model.Dto;
using Captive.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Commands.ProcessBatch
{
    public class ProcessBatchCommandHandler : IRequestHandler<ProcessBatchCommand, ProcessBatchCommandResponse>
    {

        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckOrderService _checkOrderService;
        private readonly ICheckInventoryService _checkInventoryService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IOrderFileService _orderFileService;
        private readonly IProducer<DbfGenerateMessage> _producer;

        public ProcessBatchCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, ICheckOrderService checkOrderService, ICheckInventoryService checkInventoryService, IReportGenerator reportGenerator, IOrderFileService orderFileService, IProducer<DbfGenerateMessage> producer  )
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkOrderService = checkOrderService;
            _checkInventoryService = checkInventoryService;
            _reportGenerator = reportGenerator;
            _orderFileService = orderFileService;
            _producer = producer;
        }

        public async Task<ProcessBatchCommandResponse> Handle(ProcessBatchCommand request, CancellationToken cancellationToken)
        {
            var returningObj = new ProcessBatchCommandResponse { };
            var orderFiles = await _readUow.OrderFiles
                .GetAll()
                .Include(x => x.BatchFile)
                .Include(x => x.FloatingCheckOrders)
                .Include(x => x.Product)
                .OrderBy(x => x.Product.ProductSequence)
                .Where(x => x.BatchFileId == request.BatchId).ToListAsync(cancellationToken);

            if (orderFiles == null || orderFiles.Count <= 0) 
            {
                return returningObj;
            }

            foreach (var orderFile in orderFiles) 
            {
                await _checkOrderService.CreateCheckOrder(orderFile, cancellationToken);
                var logDto = await _checkInventoryService.ApplyCheckInventory(orderFile, cancellationToken);
                await _orderFileService.UpdateOrderFileStatus(orderFile.Id, Data.Enums.OrderFilesStatus.Completed, cancellationToken);

                if (!string.IsNullOrEmpty(logDto.LogMessage))
                {
                    returningObj.LogMessage = logDto.LogMessage;
                    returningObj.LogType = logDto.LogType;
                }
                
                await _writeUow.Complete();
            }

            await _reportGenerator.OnGenerateReport(request.BatchId, cancellationToken);


            _producer.ProduceMessage(new DbfGenerateMessage
            {
                BatchId = request.BatchId
            });

            return returningObj;
        }
    }
}
