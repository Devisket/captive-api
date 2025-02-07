﻿using Captive.Applications.CheckInventory.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Commands.ProcessBatch
{
    public class ProcessBatchCommandHandler : IRequestHandler<ProcessBatchCommand, Unit>
    {

        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckOrderService _checkOrderService;
        private readonly ICheckInventoryService _checkInventoryService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IOrderFileService _orderFileService;

        public ProcessBatchCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, ICheckOrderService checkOrderService, ICheckInventoryService checkInventoryService, IReportGenerator reportGenerator, IOrderFileService orderFileService)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkOrderService = checkOrderService;
            _checkInventoryService = checkInventoryService;
            _reportGenerator = reportGenerator;
            _orderFileService = orderFileService;
        }

        public async Task<Unit> Handle(ProcessBatchCommand request, CancellationToken cancellationToken)
        {

            var orderFiles = await _readUow.OrderFiles
                .GetAll()
                .Include(x => x.BatchFile)
                .Include(x => x.FloatingCheckOrders)
                .Include(x => x.Product)
                .Where(x => x.BatchFileId == request.BatchId).ToListAsync(cancellationToken);

            if (orderFiles == null || orderFiles.Count <= 0) 
            {
                return Unit.Value;
            }

            foreach (var orderFile in orderFiles) 
            {
                await _checkOrderService.CreateCheckOrder(orderFile, cancellationToken);
                await _checkInventoryService.ApplyCheckInventory(orderFile, cancellationToken);
                await _orderFileService.UpdateOrderFileStatus(orderFile.Id, Data.Enums.OrderFilesStatus.Completed, cancellationToken);

                await _writeUow.Complete();
            }

            await _reportGenerator.OnGenerateReport(request.BatchId, cancellationToken);

            return Unit.Value;
        }
    }
}
