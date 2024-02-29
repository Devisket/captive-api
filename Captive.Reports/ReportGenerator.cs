using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Captive.Reports.PrinterFileReport;
using System.IO.Compression;

namespace Captive.Reports
{
    public class ReportGenerator : IReportGenerator
    {
        public IReadUnitOfWork _readUow { get; set; }
        public IConfiguration _configuration { get; set; }

        public IPrinterFileReport _exportPrinterFile { get; set; }

        public ReportGenerator(
            IReadUnitOfWork readUow,
            IConfiguration configuration,
            IPrinterFileReport exportPrinterFile
            )
        {
            _readUow = readUow;
            _configuration = configuration;
            _exportPrinterFile = exportPrinterFile;
        }

        public async Task OnGenerateReport(int batchFileId, CancellationToken cancellationToken)
        {
            var batchFile = await GetBatchFile( batchFileId, cancellationToken);
            var outputDir = _configuration["OutputDirectory"];
            var archiveDir = _configuration["ArchiveDirectory"];

            if (batchFile.BankInfo == null || batchFile.OrderFiles == null)
                throw new Exception("Batch file missing data");

            if(outputDir == null || archiveDir == null)
                throw new Exception("Output/Archive directory is not configured");

            var checkOrders = new List<CheckOrders>();

            foreach(var order in batchFile.OrderFiles)
            {
                var checkOrder = await GetCheckOrders(order, cancellationToken);

                checkOrders.AddRange(checkOrder);
            }
           
            var filePath = ConstructReportFolder(outputDir, batchFile.BankInfo, checkOrders);

            await _exportPrinterFile.GenerateReport(batchFile, checkOrders, filePath, cancellationToken);
            CreateZipFile(batchFile, filePath, archiveDir);
        }

        private string ConstructReportFolder(string outputDir, BankInfo bankInfo, ICollection<CheckOrders> checkOrders)
        {
            var filePath = outputDir.Replace("Bank", bankInfo.BankName);
            filePath = filePath.Replace("Date", DateTime.UtcNow.ToString("MM-dd-yyyy"));           

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            else
            {
                Directory.Delete(filePath,true);
                Directory.CreateDirectory(filePath);
            }

            var productNames = checkOrders.Select(x => x.FormChecks.ProductType.ProductName).Distinct().ToList();

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

        private async Task<BatchFile> GetBatchFile(int batchFileId, CancellationToken cancellationToken)
        {
            var batchFile = await _readUow.BatchFiles
                .GetAll()
                .Include(x => x.BankInfo)
                .Include(x => x.OrderFiles)
                .FirstOrDefaultAsync(x => x.Id == batchFileId, cancellationToken);   

            if (batchFile == null)
                throw new Exception($"No Batch file with Id: {batchFileId}");

            return batchFile;
        }

        private async Task<ICollection<CheckOrders>>GetCheckOrders(OrderFile orderFile, CancellationToken cancellationToken)
        {
            var checkOrders = await _readUow.CheckOrders.GetAll()
                .Include(x => x.FormChecks)
                .ThenInclude(x => x.ProductType)
                .Where(x => x.OrderFileId == orderFile.Id)
                .ToListAsync(cancellationToken);

            return checkOrders;
        }

        private void CreateZipFile(BatchFile file, string reportDir, string archiveDir)
        {
            if (!Directory.Exists(archiveDir))
                Directory.CreateDirectory(archiveDir);

            ZipFile.CreateFromDirectory(reportDir, Path.Combine(archiveDir, file.BatchName + ".zip"));
        }
    }
}
