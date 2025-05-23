﻿using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Captive.Utility;
using Captive.Data.Enums;

namespace Captive.Applications.Batch.Commands.CreateBatchFile
{
    public class CreateBatchFileCommandHandler : IRequestHandler<CreateBatchFileCommand, CreateBatchFileResponse>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public CreateBatchFileCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<CreateBatchFileResponse> Handle(CreateBatchFileCommand request, CancellationToken cancellationToken)
        {
            var bankInfo = await _readUow.Banks.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.BankInfoId);

            if (bankInfo == null) 
            {
                throw new Exception($"Bank Id:{request.BankInfoId} doesn't exist");
            }
                      
            var batchName = String.Format("{0}{1}",DateTime.UtcNow.ToString("MM-dd-yyyy").Replace("-",""), StringHelper.GenerateRandomChar(5));

            var batchFile = new BatchFile()
            {
                Id = Guid.NewGuid(),
                BatchName = batchName,
                OrderNumber = GetNewOrderNumber(bankInfo),
                BankInfoId = request.BankInfoId,
                CreatedDate = DateTime.UtcNow,
                BatchFileStatus = BatchFileStatus.Pending,
            };

            await _writeUow.BatchFiles.AddAsync(batchFile, cancellationToken);

            return new CreateBatchFileResponse
            {
                BankInfoId = request.BankInfoId,
                Id = batchFile.Id,
            };
        }

        public int GetNewOrderNumber(BankInfo bankInfo)
        {
            int orderNumber = 1;

            var lastBatch = bankInfo.BatchFiles?.Where(x => x.CreatedDate < DateTime.UtcNow).OrderByDescending(x => x.OrderNumber).FirstOrDefault();

            if (lastBatch != null) 
                orderNumber = lastBatch.OrderNumber + 1;
            
            return orderNumber;
        }
    }
}
