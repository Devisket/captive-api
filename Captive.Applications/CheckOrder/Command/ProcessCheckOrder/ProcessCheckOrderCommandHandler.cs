using Captive.Applications.CheckInventory.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Command.ProcessCheckOrder
{
    public class ProcessCheckOrderCommandHandler : IRequestHandler<ProcessCheckOrderCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckOrderService _checkOrderService;
        private readonly ICheckInventoryService _checkInventoryService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IOrderFileService _orderFileService;

        public ProcessCheckOrderCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, ICheckOrderService checkOrderService, ICheckInventoryService checkInventoryService, IReportGenerator reportGenerator, IOrderFileService orderFileService)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkOrderService = checkOrderService;
            _checkInventoryService = checkInventoryService;
            _reportGenerator = reportGenerator;
            _orderFileService = orderFileService;
        }

        public async Task<Unit> Handle(ProcessCheckOrderCommand request, CancellationToken cancellationToken)
        {
            /**
             * STEP TO PROCESS
             * 1. Apply check inventory into floating order files
             * 2. Create record out of Check Order table
             * 3. Generate Report
             */
            var orderFile = await _readUow.OrderFiles
                .GetAll()
                .Include(x => x.BatchFile)
                .Include(x => x.FloatingCheckOrders)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null)
                throw new SystemException($"Order file ID {request.OrderFileId} doesn't exist");

            await _checkOrderService.CreateCheckOrder(orderFile, cancellationToken);
            await _checkInventoryService.ApplyCheckInventory(orderFile, cancellationToken);
            await _orderFileService.UpdateOrderFileStatus(orderFile.Id, Data.Enums.OrderFilesStatus.Completed, cancellationToken);

            await _writeUow.Complete();

            await _reportGenerator.OnGenerateReport(orderFile.BatchFileId, cancellationToken);

            return Unit.Value;
        }
    }
}
