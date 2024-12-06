using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Model.Dto;
using Captive.Utility;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Captive.Applications.ProcessOrderFiles.Commands.UploadOrderFile
{
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UploadOrderFileCommandHandler> _logger;

        private readonly IProducer<FileUploadMessage> _producer;

        public UploadOrderFileCommandHandler(
            IReadUnitOfWork readUow,
            IWriteUnitOfWork writeUow,
            IProducer<FileUploadMessage> producer,
            IConfiguration configuration,
            ILogger<UploadOrderFileCommandHandler> logger)
        {
            _readUow = readUow;
            _writeUow = writeUow;
            _producer = producer;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Unit> Handle(UploadOrderFileCommand request, CancellationToken cancellationToken)
        {
            var bankInfo = await _readUow.Banks.GetAll().FirstOrDefaultAsync(x => x.Id == request.BankId);

            var batch = await _readUow.BatchFiles.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.BatchId);

            if (bankInfo == null)
                throw new Exception($"the bankId: {request.BankId} doesn't exist");

            if (batch == null)
                throw new Exception($"Batch ID:{request.BatchId} doesn't exist");

            if (!request.Files.Any())
            {
                throw new Exception($"File can't be empty");
            }

            var dirPath = CreateDirectory(bankInfo.ShortName, batch.BatchName);

            var orderFiles = await ProcessFiles(batch.Id, request.Files, dirPath, cancellationToken);

            await _writeUow.Complete();

            _producer.ProduceMessage(new FileUploadMessage
            {
                BankId = request.BankId,
                BatchID = batch.Id,
                BatchDirectory = dirPath,
                Files = orderFiles.Select(x => new OrderfileDto {
                    Id = x.Id,
                    BatchId = x.BatchFileId,
                    FileName = x.FileName,
                    FileType = Path.GetExtension(x.FileName).SanitizeFileName(),
                    FilePath = x.FilePath,
                    Status = x.Status.ToString(),
                    ProductId = x.ProductId
                })
            });

            return Unit.Value;
        }

        private async Task<List<OrderFile>> ProcessFiles(Guid batchId, IEnumerable<IFormFile> files, string directoryPath, CancellationToken cancellationToken)
        {
            var rootPath = Path.Combine(_configuration["Processing:FileProcessing"], directoryPath);

            List<OrderFile> result = new List<OrderFile>();

            foreach (var file in files) 
            { 
                await SaveFile(file, rootPath, cancellationToken);

                var orderFile = await CreateOrderFileRecord(batchId, file.FileName, directoryPath, cancellationToken);

                result.Add(orderFile);
            }
            return result;
        }

        private async Task<OrderFile> CreateOrderFileRecord(Guid batchId, string fileName, string batchDirectory, CancellationToken cancellationToken )
        {
            var relativePath = batchDirectory.Split("processing")[1];

            var productId = await GetProductId(fileName, cancellationToken);

            var orderFile = new OrderFile
            {
                Id = Guid.NewGuid(),
                BatchFileId = batchId,
                FileName = fileName,
                FilePath = Path.Combine(relativePath, fileName),
                Status = Data.Enums.OrderFilesStatus.Processing,
                ProductId = productId,
                ProcessDate = DateTime.UtcNow,
            };

            await _writeUow.OrderFiles.AddAsync(orderFile, cancellationToken);

            return orderFile;
        }

        private async Task SaveFile(IFormFile file, string directoryPath,CancellationToken cancellationToken)
        {
            var fileBytes = await ExtractFile(file, cancellationToken);       
            await File.WriteAllBytesAsync(Path.Combine(directoryPath, file.FileName), fileBytes, cancellationToken);            
        }

        private string CreateDirectory(string bankShortName, string batchName)
        {
            var rootPath = _configuration["Processing:FileProcessing"];

            rootPath = rootPath.Replace("bankShortName", bankShortName);
            rootPath = rootPath.Replace("currentDate", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            rootPath = rootPath.Replace("batchName", batchName);


            if (string.IsNullOrEmpty(rootPath))
                throw new Exception("File processing directory is not defined");

            Directory.CreateDirectory(rootPath);

            return rootPath;
        }

        private async Task<byte[]> ExtractFile(IFormFile rawFile, CancellationToken cancellationToken)
        {
            using (var fileStream = rawFile.OpenReadStream())
            {
                byte[] fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length, cancellationToken);
                fileStream.Close();

                return fileBytes;
            }
        }      

        private async Task<Guid> GetProductId(string fileName, CancellationToken cancellationToken)
        {
            var productConfiguration = await _readUow.ProductConfigurations.GetAll().FirstOrDefaultAsync(x => fileName.Contains(x.FileName), cancellationToken);

            if (productConfiguration == null)
                throw new Exception($"File name {fileName} has no product configuration");

            return productConfiguration.ProductId;
        }
    }
}
