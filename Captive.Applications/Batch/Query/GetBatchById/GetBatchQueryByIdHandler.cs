﻿using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Query.GetBatchById
{
    public class GetBatchQueryByIdHandler : IRequestHandler<GetBatchByIdQuery, GetBatchByIdQueryResponse>
    {
        public IReadUnitOfWork _readUow;

        public GetBatchQueryByIdHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetBatchByIdQueryResponse> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _readUow.BatchFiles.GetAll().Where(x => x.Id == request.BatchId);

            var batch = await _readUow.BatchFiles.GetAll()
                .Where(x => x.BankInfoId == request.BankId)
                .Select(x => new GetBatchByIdQueryResponse()
            {
                Id = x.Id,
                BatchFileStatus = x.BatchFileStatus,
                BatchName = x.BatchName,
                CreatedDate = x.CreatedDate,
                OrderNumber = x.OrderNumber,
                OrderFiles = x.OrderFiles != null && x.OrderFiles.Any() ? x.OrderFiles.Select(x => new OrderfileDto
                {
                    Id = x.Id,
                    BatchId = x.BatchFileId,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Status = x.Status.ToString()
                }).ToList() : null
            }).FirstOrDefaultAsync();

            return batch;
        }
    }
}
