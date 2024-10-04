﻿using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Model.Dto;
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
                    FilePath = x.FilePath,
                    Status = x.Status.ToString(),
                })
            });

            return Unit.Value;
        }

        private async Task<List<OrderFile>> ProcessFiles(Guid batchId, IEnumerable<IFormFile> files, string directoryPath, CancellationToken cancellationToken)
        {
            List<OrderFile> result = new List<OrderFile>();

            foreach (var file in files) 
            { 
                await SaveFile(file, directoryPath, cancellationToken);

                var orderFile = await CreateOrderFileRecord(batchId, file.FileName, directoryPath, cancellationToken);

                result.Add(orderFile);
            }
            return result;
        }

        private async Task<OrderFile> CreateOrderFileRecord(Guid batchId, string fileName, string batchDirectory, CancellationToken cancellationToken )
        {
            var orderFile = new OrderFile
            {
                Id = Guid.NewGuid(),
                BatchFileId = batchId,
                FileName = fileName,
                FilePath = batchDirectory + fileName,
                Status = Data.Enums.OrderFilesStatus.Processing,
                ProcessDate = DateTime.UtcNow,
            };

            await _writeUow.OrderFiles.AddAsync(orderFile, cancellationToken);

            return orderFile;
        }

        private async Task SaveFile(IFormFile file, string directoryPath,CancellationToken cancellationToken)
        {
            var fileBytes = await ExtractFile(file, cancellationToken);       
            await File.WriteAllBytesAsync(directoryPath + file.FileName, fileBytes, cancellationToken);         
        }

        private string CreateDirectory(string bankShortName, string batchName)
        {
            var directoryPath = _configuration["Processing:FileProcessing"];

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("File processing directory is not defined");
            
            directoryPath = directoryPath.Replace("bankShortName", bankShortName);
            directoryPath = directoryPath.Replace("currentDate", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            directoryPath = directoryPath.Replace("batchName", batchName);

            Directory.CreateDirectory(directoryPath);

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
