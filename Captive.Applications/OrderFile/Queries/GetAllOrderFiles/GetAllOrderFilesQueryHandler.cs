using Captive.Applications.OrderFile.Queries.GetAllOrderFiles.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.OrderFile.Queries.GetAllOrderFiles
{
    public class GetAllOrderFilesQueryHandler : IRequestHandler<GetAllOrderFilesQuery, GetAllOrderFilesQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetAllOrderFilesQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetAllOrderFilesQueryResponse> Handle(GetAllOrderFilesQuery request, CancellationToken cancellationToken)
        {
            var batchFiles = await _readUow.BatchFiles.GetAll().Where(x => x.BankInfoId == request.BankId)
                .Select(x => new BatchFileDtoResponse { 
                Id = x.Id,
                UploadDate = x.UploadDate,
                }).ToListAsync(cancellationToken);

            if(batchFiles != null && batchFiles.Any() )
            {
                var orderFiles = await _readUow.OrderFiles.GetAll()
                    .Include(x => x.CheckOrders)
                    .Where(x => batchFiles.Any(z => z.Id == x.BatchFileId))
                    .Select(x => new OrderFileDtoResponse
                    {
                        Id = x.Id,
                        BatchFileId = x.BatchFileId,
                        FileName = x.FileName,
                        FileStatus = x.Status,
                        CheckOrders = x.CheckOrders.Select(z => new CheckOrderDtoResponse
                        {
                            Id = z.Id,
                            AccountName = z.AccountName,
                            BRSTN = z.BRSTN,
                            DeliveringBRSTN = z.DeliverTo,
                            Quantity = z.OrderQuanity
                        }).ToList()
                    }).ToListAsync();

                batchFiles = batchFiles.Select(x => new BatchFileDtoResponse
                {
                    Id = x.Id,
                    UploadDate = x.UploadDate,
                    OrderFiles = orderFiles.Where(z => z.BatchFileId == x.Id).ToList()
                }).ToList();
            }

            throw new NotImplementedException();
        }
    }
}
