using Captive.Applications.Batch.Hubs;
using Captive.Applications.CheckInventory.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Notifications;
using Captive.Reports;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Captive.Applications.Batch.Services
{
    public interface IBatchProcessingOrchestratorService
    {
        Task ProcessAsync(Guid jobId, Guid batchId, bool forceProcess, CancellationToken cancellationToken);
    }

    public class BatchProcessingOrchestratorService : IBatchProcessingOrchestratorService
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly ICheckOrderService _checkOrderService;
        private readonly ICheckInventoryService _checkInventoryService;
        private readonly IOrderFileService _orderFileService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IHubContext<BatchHub> _hubContext;
        private readonly IOrderFileNotifier _orderFileNotifier;

        public BatchProcessingOrchestratorService(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow,
            ICheckOrderService checkOrderService,
            ICheckInventoryService checkInventoryService,
            IOrderFileService orderFileService,
            IReportGenerator reportGenerator,
            IHubContext<BatchHub> hubContext,
            IOrderFileNotifier orderFileNotifier)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _checkOrderService = checkOrderService;
            _checkInventoryService = checkInventoryService;
            _orderFileService = orderFileService;
            _reportGenerator = reportGenerator;
            _hubContext = hubContext;
            _orderFileNotifier = orderFileNotifier;
        }

        public async Task ProcessAsync(Guid jobId, Guid batchId, bool forceProcess, CancellationToken cancellationToken)
        {
            var job = await _writeUow.BatchJobs.GetAll()
                .FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);

            if (job == null) return;

            try
            {
                await UpdateJob(job, BatchJobStatus.Running, 0, "Loading order files", cancellationToken);

                var orderFiles = await _readUow.OrderFiles.GetAll()
                    .Include(x => x.BatchFile)
                    .Include(x => x.FloatingCheckOrders)
                    .Include(x => x.Product)
                        .ThenInclude(x => x.BankInfo)
                    .Where(x => x.BatchFileId == batchId)
                    .OrderBy(x => x.Product.ProductSequence)
                    .ToListAsync(cancellationToken);

                if (orderFiles == null || orderFiles.Count == 0)
                {
                    await UpdateJob(job, BatchJobStatus.Failed, 0, null, cancellationToken, "No order files found for this batch.");
                    return;
                }

                int totalFiles = orderFiles.Count;
                int perFileProgress = totalFiles > 0 ? 70 / totalFiles : 70;

                await UpdateJob(job, BatchJobStatus.Running, 10, "Creating check orders", cancellationToken);

                for (int i = 0; i < orderFiles.Count; i++)
                {
                    var orderFile = orderFiles[i];
                    var stepDetail = $"Creating check orders ({i + 1} of {totalFiles})";
                    await _orderFileNotifier.NotifyOrderFileProgress(batchId, orderFile.Id, stepDetail, cancellationToken);
                    await _checkOrderService.CreateCheckOrder(orderFile, cancellationToken);

                    int progress = 10 + ((i + 1) * perFileProgress / 2);
                    await UpdateJob(job, BatchJobStatus.Running, progress, stepDetail, cancellationToken);
                }

                // Check inventory warnings right before applying — interrupt here if needed
                var inventoryWarnings = await _checkInventoryService.GetInventoryWarnings(orderFiles, cancellationToken);
                if (inventoryWarnings.Count > 0)
                {
                    job.Warnings = JsonConvert.SerializeObject(inventoryWarnings);
                    await UpdateJob(job, BatchJobStatus.AwaitingConfirmation, job.Progress, "Awaiting confirmation", cancellationToken);

                    bool confirmed = await WaitForUserConfirmation(job.Id, cancellationToken);
                    if (!confirmed) return;

                    await UpdateJob(job, BatchJobStatus.Running, job.Progress, "Applying inventory", cancellationToken);
                }

                await UpdateJob(job, BatchJobStatus.Running, 45, "Applying inventory", cancellationToken);

                for (int i = 0; i < orderFiles.Count; i++)
                {
                    var orderFile = orderFiles[i];
                    var stepDetail = $"Applying inventory ({i + 1} of {totalFiles})";
                    await _orderFileNotifier.NotifyOrderFileProgress(batchId, orderFile.Id, stepDetail, cancellationToken);
                    await _checkInventoryService.ApplyCheckInventory(orderFile, cancellationToken);
                    await _orderFileService.UpdateOrderFileStatus(orderFile.Id, OrderFilesStatus.GeneratingReport, cancellationToken);
                    await _writeUow.Complete(cancellationToken);

                    int progress = 45 + ((i + 1) * perFileProgress / 2);
                    await UpdateJob(job, BatchJobStatus.Running, progress, stepDetail, cancellationToken);
                }

                await UpdateJob(job, BatchJobStatus.Running, 90, "Generating report", cancellationToken);
                await _orderFileNotifier.NotifyBatchProgress(batchId, "Generating barcodes...", cancellationToken);

                await _reportGenerator.GenerateBarcode(
                    orderFiles.First().Product.BankInfo!,
                    orderFiles.First().BatchFileId,
                    cancellationToken);

                await UpdateJob(job, BatchJobStatus.Completed, 100, "Completed", cancellationToken);
            }
            catch (Exception ex)
            {
                var job2 = await _writeUow.BatchJobs.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);

                if (job2 != null)
                    await UpdateJob(job2, BatchJobStatus.Failed, job2.Progress, job2.CurrentStep, cancellationToken, ex.Message);
            }
        }

        private async Task<bool> WaitForUserConfirmation(Guid jobId, CancellationToken cancellationToken)
        {
            var deadline = DateTime.UtcNow.AddMinutes(30);
            while (!cancellationToken.IsCancellationRequested && DateTime.UtcNow < deadline)
            {
                await Task.Delay(1000, cancellationToken);
                var latest = await _writeUow.BatchJobs.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);

                if (latest == null || latest.Status == BatchJobStatus.Failed) return false;
                if (latest.ForceProcess) return true;
            }
            return false;
        }

        private async Task UpdateJob(BatchJob job, BatchJobStatus status, int progress, string? currentStep, CancellationToken cancellationToken, string? errorMessage = null)
        {
            job.Status = status;
            job.Progress = progress;
            job.CurrentStep = currentStep;
            job.UpdatedAt = DateTime.UtcNow;
            if (errorMessage != null)
                job.ErrorMessage = errorMessage;
            await _writeUow.Complete(cancellationToken);
            await BroadcastJob(job);
        }

        private async Task BroadcastJob(BatchJob job)
        {
            var payload = new
            {
                jobId = job.Id,
                batchId = job.BatchId,
                status = job.Status.ToString(),
                progress = job.Progress,
                currentStep = job.CurrentStep,
                warnings = job.Warnings != null
                    ? JsonConvert.DeserializeObject<List<string>>(job.Warnings)
                    : new List<string>(),
                errorMessage = job.ErrorMessage,
            };

            await _hubContext.Clients.All.SendAsync($"batchJob:{job.BatchId}", payload);
        }
    }
}
