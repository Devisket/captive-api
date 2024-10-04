using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Query
{
    public class GetBatchQueryHandler : IRequestHandler<GetBatchQuery, GetBatchQueryResponse>
    {
        public IReadUnitOfWork _readUow;

        public GetBatchQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetBatchQueryResponse> Handle(GetBatchQuery request, CancellationToken cancellationToken)
        {
            var batches = await _readUow.BatchFiles.GetAll().Where(x => x.BankInfoId == request.BankId).Select(x => new BatchFileDto()
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
            }).ToListAsync();

            return new GetBatchQueryResponse
            {
                BatchFiles = batches,
            };
        }
    }
}
