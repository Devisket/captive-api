using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Captive.Utility;
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
            var batch = await _readUow.BatchFiles.GetAll()
                .Where(x => x.BankInfoId == request.BankId && x.Id == request.BatchId)
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
                    FileType = Path.GetExtension(x.FileName).SanitizeFileName(),
                    FilePath = x.FilePath,
                    Status = x.Status.ToString(),
                    CheckOrders = x.FloatingCheckOrders != null && x.FloatingCheckOrders.Any() ? x.FloatingCheckOrders.Select(c => new CheckOrderDto
                    {
                        Id = c.Id,
                        BRSTN = c.BRSTN,                        
                        AccountNumber = c.AccountNo,
                        MainAccountName = c.AccountName,
                        DeliverTo = c.DeliverTo,
                        FormType = c.FormType,
                        CheckType = c.CheckType,
                        Quantity = c.Quantity,                  
                    }).ToList() : null
                }).ToList() : null
            }).FirstOrDefaultAsync();

            return batch;
        }
    }
}
