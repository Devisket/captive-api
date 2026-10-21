using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Captive.Reports.PrinterFileReport;
using System.IO.Compression;
using Captive.Reports.BlockReport;
using Captive.Reports.PackingReport;
using Captive.Reports.HashTotalReport;
using Captive.Model.Dto;
using Captive.Model.Notifications;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Data.UnitOfWork.Write;
using Captive.Data.Enums;
using Captive.Model.Dto.Reports;

namespace Captive.Reports
{
    public class ReportGenerator : IReportGenerator
    {
        // Maximum number of booklets of a single check order that goes into one generated file
        // when the order file has split generation enabled.
        private const int SplitGenerationLimit = 50;

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IConfiguration _configuration;
        private readonly IBlockReport _blockReport;
        private readonly IPrinterFileReport _exportPrinterFile;
        private readonly IPackingReport _packingReport;
        private readonly IProducer<GenerateBarcodeMessage> _producerGenerateBarcode;
        private readonly IProducer<DbfGenerateMessage> _producerGenerateDbf;
        private readonly IOrderFileNotifier _orderFileNotifier;
        private readonly IHashTotalReport _hashTotalReport;
        private readonly IReportService _reportService;

        public ReportGenerator(
            IReadUnitOfWork readUow,
            IWriteUnitOfWork writeUow,
            IConfiguration configuration,
            IPrinterFileReport exportPrinterFile,
            IBlockReport blockReport,
            IPackingReport packingReport,
            IProducer<GenerateBarcodeMessage> producerGenerateBarcode,
            IProducer<DbfGenerateMessage> producerGenerateDbf,
            IOrderFileNotifier orderFileNotifier,
            IHashTotalReport hashTotalReport,
            IReportService reportService
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
            _orderFileNotifier = orderFileNotifier;
            _hashTotalReport = hashTotalReport;
            _reportService = reportService;
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

            var checkDto = await _reportService.ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

            ApplySplitGeneration(checkDto);

            await _orderFileNotifier.NotifyBatchProgress(batchFileId, "Generating Printer File Report", cancellationToken);
            await _exportPrinterFile.GenerateReport(batchFile, checkDto, filePath, cancellationToken);

            await _orderFileNotifier.NotifyBatchProgress(batchFileId, "Generating Block Report", cancellationToken);
            await _blockReport.GenerateReport(batchFile, checkDto, filePath, cancellationToken);

            await _orderFileNotifier.NotifyBatchProgress(batchFileId, "Generating Packing Report", cancellationToken);
            await _packingReport.GenerateReport(batchFile, checkDto, filePath, cancellationToken);

            await _orderFileNotifier.NotifyBatchProgress(batchFileId, "Generating Hash Total Report", cancellationToken);
            await _hashTotalReport.GenerateReport(batchFile, checkDto, filePath, cancellationToken);

            //CreateZipFile(batchFile, filePath, archiveDir);

            await _orderFileNotifier.NotifyBatchProgress(batchFileId, "Generating DBF File", cancellationToken);
            _producerGenerateDbf.ProduceMessage(new DbfGenerateMessage
            {
                BatchId = batchFileId,
                outputFolder = filePath
            });

        }

        /// <summary>
        /// Assigns the file suffix of every booklet when split generation is enabled on its order file.
        /// Each check order is chunked by <see cref="SplitGenerationLimit"/> booklets: the first chunk goes to
        /// file 1 together with the other check orders, the next chunk to file 2, and so on.
        /// e.g. orders 1 (10), 2 (120) and 3 (5) produce file 1 = 1, 2 (first 50), 3; file 2 = 2 (next 50);
        /// file 3 = 2 (last 20). Suffixes are only applied to a product/form check group that actually
        /// needs more than one file, so the file name stays unchanged (e.g. CWS98C) when nothing is split.
        /// </summary>
        private static void ApplySplitGeneration(ICollection<CheckOrderReport> checkDto)
        {
            var reportGroups = checkDto.GroupBy(x => new { x.ProductTypeName, x.FormCheckName, x.CustomizeFileName });

            foreach (var reportGroup in reportGroups)
            {
                var partIndexes = new Dictionary<CheckOrderReport, int>();

                foreach (var checkOrderBooklets in reportGroup.GroupBy(x => x.CheckOrder.Id))
                {
                    var isSplitGeneration = checkOrderBooklets.First().CheckOrder.OrderFile?.IsSplitGeneration ?? false;

                    var booklets = checkOrderBooklets
                        .OrderBy(x => x.StartSeries.Length)
                        .ThenBy(x => x.StartSeries, StringComparer.Ordinal)
                        .ToList();

                    for (int i = 0; i < booklets.Count; i++)
                    {
                        partIndexes[booklets[i]] = isSplitGeneration ? i / SplitGenerationLimit : 0;
                    }
                }

                var hasSplit = partIndexes.Values.Any(x => x > 0);

                foreach (var (booklet, partIndex) in partIndexes)
                {
                    booklet.FileSuffix = hasSplit ? (partIndex + 1).ToString() : string.Empty;
                }
            }
        }

        private string ConstructReportFolder(string outputDir, BankInfo bankInfo, ICollection<CheckOrders> checkOrders, string batchName)
        {
            var dateDir = outputDir.Replace("bankShortName", bankInfo.ShortName);
            dateDir = dateDir.Replace("currentDate", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            dateDir = dateDir.Substring(0, dateDir.IndexOf("batchName", StringComparison.Ordinal));

            var folderName = GetBatchFolderName(dateDir, batchName);
            var filePath = Path.Combine(dateDir, folderName);

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

        // Labels each batch's report folder with a trailing letter (A, B, C, ...) so multiple
        // batches generated for the same bank/date remain distinguishable. Re-generating a report
        // for the same batch reuses its existing lettered folder instead of allocating a new one.
        private static string GetBatchFolderName(string dateDir, string batchName)
        {
            if (!Directory.Exists(dateDir))
                return $"{batchName}_A";

            var existingFolders = Directory.GetDirectories(dateDir)
                .Select(Path.GetFileName)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var existingFolderForBatch = existingFolders
                .FirstOrDefault(x => x!.StartsWith($"{batchName}_", StringComparison.OrdinalIgnoreCase));

            if (existingFolderForBatch != null)
                return existingFolderForBatch;

            return $"{batchName}_{GetNextBatchLetter(existingFolders.Count)}";
        }

        private static string GetNextBatchLetter(int existingFolderCount)
        {
            var ordinal = existingFolderCount + 1;
            var letters = string.Empty;

            while (ordinal > 0)
            {
                var remainder = (ordinal - 1) % 26;
                letters = (char)('A' + remainder) + letters;
                ordinal = (ordinal - 1) / 26;
            }

            return letters;
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
