using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Captive.Applications.OrderFile.Commands.UploadOrderFile
{
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand,Unit>
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

            await SaveFile(request.Files, dirPath, cancellationToken);

            _producer.ProduceMessage(new FileUploadMessage
            {
                BankId = request.BankId,
                BatchID = Guid.NewGuid(),
                Files = request.Files.Select(x => x.FileName)
            });

            return Unit.Value;
        }

        private async Task SaveFile(IEnumerable<IFormFile> files, string directoryPath,CancellationToken cancellationToken)
        {
            foreach (var file in files) 
            {
                var fileBytes = await ExtractFile(file, cancellationToken);       

                await File.WriteAllBytesAsync(directoryPath + file.FileName, fileBytes, cancellationToken);
            }
        }

        private string CreateDirectory(string bankShortName, string batchName)
        {
            var directoryPath = _configuration["Processing:FileProcessing"];

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("File processing directory is not defined");
            
            directoryPath = directoryPath.Replace("bankShortName", bankShortName);
            directoryPath = directoryPath.Replace("currentDate", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            directoryPath = directoryPath.Replace("batchName", batchName);

            return directoryPath;
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
    }
}
