using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Query.GetAllBatch
{
    public class GetBatchQueryHandler : IRequestHandler<GetBatchQuery, ICollection<BatchFilesDto>>
    {
        public IReadUnitOfWork _readUow;

        public GetBatchQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<ICollection<BatchFilesDto>> Handle(GetBatchQuery request, CancellationToken cancellationToken)
        {
            var query = _readUow.BatchFiles.GetAll();

            var batches = await _readUow.BatchFiles.GetAll()
                .Include(x => x.OrderFiles)
                .Where(x => x.BankInfoId == request.BankId)
                .Select(x => new BatchFilesDto()
            {
                Id = x.Id,
                BatchFileStatus = x.BatchFileStatus.ToString(),
                BatchName = x.BatchName,
                CreatedDate = x.CreatedDate.ToString("MM-dd-yyyy"),
                NoOfFiles = x.OrderFiles.Count(),
                OrderNumber = x.OrderNumber,
                ErrorMessage = x.ErrorMessage,
                DeliveryDate = x.DeliveryDate
            }).ToListAsync();

            return batches;
        }
    }
}
