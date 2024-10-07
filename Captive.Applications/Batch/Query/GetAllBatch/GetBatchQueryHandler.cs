using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Query.GetAllBatch
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
            var query = _readUow.BatchFiles.GetAll();

            var batches = await _readUow.BatchFiles.GetAll()
                .Where(x => x.BankInfoId == request.BankId)
                .Select(x => new BatchFilesDto()
            {
                Id = x.Id,
                BatchFileStatus = x.BatchFileStatus,
                BatchName = x.BatchName,
                CreatedDate = x.CreatedDate,
                OrderNumber = x.OrderNumber
            }).ToListAsync();

            return new GetBatchQueryResponse
            {
                BatchFiles = batches,
            };
        }
    }
}
