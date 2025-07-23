using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Captive.Reports.PrinterFileReport;
using System.IO.Compression;
using Captive.Reports.BlockReport;
using Captive.Reports.PackingReport;
using Captive.Model.Dto;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Data.UnitOfWork.Write;
using Captive.Data.Enums;

namespace Captive.Reports
{
    public class ReportGenerator : IReportGenerator
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IConfiguration _configuration;
        private readonly IBlockReport _blockReport;
        private readonly IPrinterFileReport _exportPrinterFile;
        private readonly IPackingReport _packingReport;
        private readonly IProducer<GenerateBarcodeMessage> _producerGenerateBarcode;
        private readonly IProducer<DbfGenerateMessage> _producerGenerateDbf;

        public ReportGenerator(
            IReadUnitOfWork readUow,
            IWriteUnitOfWork writeUow,
            IConfiguration configuration,
            IPrinterFileReport exportPrinterFile,
            IBlockReport blockReport,
            IPackingReport packingReport,
            IProducer<GenerateBarcodeMessage> producerGenerateBarcode,
            IProducer<DbfGenerateMessage> producerGenerateDbf
            )
        {
            _readUow = readUow;
            _writeUow = writeUow;
            _configuration = configuration;
            _exportPrinterFile = exportPrinterFile;
            _blockReport = blockReport;
            _packingReport = packingReport;
            _producerGenerateBarcode = producerGenerateBarcode;
            _producerGenerateDbf = producerGenerateDbf;
        }

        public async Task OnGenerateReport(Guid batchFileId, CancellationToken cancellationToken)
        {
            var batchFile = await GetBatchFile(batchFileId, cancellationToken);

            if (batchFile == null)
                return;

            if (!batchFile.OrderFiles!.Any(x => x.Status == Data.Enums.OrderFilesStatus.GeneratingReport))
            {
                return;
            }

            var outputDir = _configuration["Processing:OutputDirectory"];
            var archiveDir = _configuration["Processing:ArchiveDirectory"];

            if (batchFile.BankInfo == null || batchFile.OrderFiles == null)
                throw new Exception("Batch file missing data");

            if (outputDir == null || archiveDir == null)
                throw new Exception("Output/Archive directory is not configured");

            var checkOrders = new List<CheckOrders>();

            foreach (var order in batchFile.OrderFiles.Where(x => x.Status == Data.Enums.OrderFilesStatus.GeneratingReport))
            {
                var checkOrder = await GetCheckOrders(order, cancellationToken);

                checkOrders.AddRange(checkOrder);
            }

            var filePath = ConstructReportFolder(outputDir, batchFile.BankInfo, checkOrders,batchFile.BatchName);

            await _exportPrinterFile.GenerateReport(batchFile, checkOrders, filePath, cancellationToken);
            await _blockReport.GenerateReport(batchFile, checkOrders, filePath, cancellationToken);
            await _packingReport.GenerateReport(batchFile, checkOrders, filePath, cancellationToken);

            //CreateZipFile(batchFile, filePath, archiveDir);

            _producerGenerateDbf.ProduceMessage(new DbfGenerateMessage
            {
                BatchId = batchFileId
            });

        }

        private string ConstructReportFolder(string outputDir, BankInfo bankInfo, ICollection<CheckOrders> checkOrders, string batchName)
        {
            var filePath = outputDir.Replace("bankShortName", bankInfo.ShortName);
            filePath = filePath.Replace("currentDate", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            filePath = filePath.Replace("batchName", batchName);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            else
            {
                Directory.Delete(filePath, true);
                Directory.CreateDirectory(filePath);
            }

            var productNames = checkOrders.Select(x => x.OrderFile.Product.ProductName).Distinct().ToList();

            foreach (var productName in productNames)
            {
                var subDir = Path.Combine(filePath, productName);

                if (!Directory.Exists(subDir))
                {
                    Directory.CreateDirectory(subDir);
                }
            }

            return filePath;
        }

        private async Task<BatchFile> GetBatchFile(Guid batchFileId, CancellationToken cancellationToken)
        {
            var batchFile = await _readUow.BatchFiles
                .GetAll()
                .Include(x => x.BankInfo)
                .Include(x => x.OrderFiles)
                    .ThenInclude(x => x.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == batchFileId, cancellationToken);

            if (batchFile == null)
                throw new Exception($"No Batch file with Id: {batchFileId}");

            return batchFile;
        }

        private async Task<ICollection<CheckOrders>> GetCheckOrders(OrderFile orderFile, CancellationToken cancellationToken)
        {
            var checkOrders = await _readUow.CheckOrders.GetAll()
                .Include(x => x.OrderFile)
                .Include(x => x.Product)
                .Include(x => x.CheckInventoryDetail)
                .Where(x => x.OrderFileId == orderFile.Id)
                .ToListAsync(cancellationToken);

            return checkOrders;
        }

        private void CreateZipFile(BatchFile file, string reportDir, string archiveDir)
        {
            if (!Directory.Exists(archiveDir))
                Directory.CreateDirectory(archiveDir);

            var fileName = Path.Combine(archiveDir, String.Format("{0}_{1}", file.BatchName, file.OrderNumber.ToString()) + ".zip");

            if (File.Exists(fileName))
                File.Delete(fileName);

            ZipFile.CreateFromDirectory(reportDir, fileName);
        }

        public async Task GenerateBarcode(BankInfo bankInfo, Guid batchFileId, CancellationToken cancellationToken)
        {
            var checkOrders = await _readUow.CheckOrders.GetAll()
                .Include(x => x.CheckInventoryDetail)
                .Include(x => x.OrderFile)
                .Where(x => 
                    x.OrderFile.BatchFileId == batchFileId && 
                    x.OrderFile.Status == OrderFilesStatus.GeneratingReport)
                .ToListAsync(cancellationToken);

            if (checkOrders.Count <= 0)
                return;

            var checkOrderWithoutBarcode = checkOrders
                .Where(x => string.IsNullOrEmpty(x.BarCodeValue))
                .Select(x => new CheckOrderBarcodeDto
            {
                AccountNumber = x.AccountNo,
                BRSTN = x.BRSTN,
                CheckOrderId = x.Id,
                Quantity = x.Quantity,
                CheckInventories = x.CheckInventoryDetail!.OrderBy(x => x.StartingSeries).Select(x => new CheckInventoryDetailBarcodeDto 
                { 
                    CheckInventoryDetailId = x.Id,
                    StartingSeries = x.StartingSeries!,
                    EndingSeries = x.EndingSeries!
                }).ToList(),
            });

            if (!checkOrderWithoutBarcode.Any())
            {
                await OnGenerateReport(batchFileId, cancellationToken);
            }
            else
            {
                _producerGenerateBarcode.ProduceMessage(new GenerateBarcodeMessage
                {
                    BankId = bankInfo.Id,
                    BatchId = batchFileId,
                    BarcodeService = bankInfo.BarcodeService ?? string.Empty,
                    CheckOrderBarcode = checkOrderWithoutBarcode,
                });
            }
        }
    }
}
