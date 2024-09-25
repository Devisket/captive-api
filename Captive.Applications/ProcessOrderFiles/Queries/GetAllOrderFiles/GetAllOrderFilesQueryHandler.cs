using Captive.Applications.ProcessOrderFiles.Queries.GetAllOrderFiles.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.ProcessOrderFiles.Queries.GetAllOrderFiles
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
            var batchFiles = _readUow.BatchFiles.GetAll().Where(x => x.BankInfoId == request.BankId);

            if (batchFiles != null && batchFiles.Any())
            {
                var orderFiles = await _readUow.OrderFiles.GetAll()
                    .Include(x => x.BatchFile)
                    .Where(x => batchFiles.Any(z => z.Id == x.BatchFileId))
                    .Select(x => new OrderFileDtoResponse
                    {
                        Id = x.Id,
                        BatchFileId = x.BatchFileId,
                        FileName = x.FileName.Trim(),
                        FileStatus = x.Status.ToString()
                    }).ToListAsync();


                foreach (var orderFile in orderFiles)
                {
                   var checkOrders = await _readUow.CheckOrders.GetAll().AsNoTracking().Where(x => x.OrderFileId == orderFile.Id).ToListAsync(cancellationToken);

                    if (checkOrders.Any())
                    {
                        orderFile.CheckOrders = checkOrders.Select(x => new CheckOrderDtoResponse
                        {
                            AccountName = x.AccountName.Trim(),
                            BRSTN = x.BRSTN,
                            DeliveringBRSTN = x.DeliverTo,
                            Id = x.Id,
                            Quantity = x.OrderQuanity
                        }).ToList();
                    }
                }


                var returnObj = new GetAllOrderFilesQueryResponse
                {
                    BankId = request.BankId,
                    Batches = await batchFiles.Select(x =>
                    new BatchFileDtoResponse
                    {
                        Id = x.Id,
                        CreatedDate = x.CreatedDate,
                    }).ToListAsync()
                };

                foreach(var batchFile in returnObj.Batches)
                {
                    batchFile.OrderFiles = orderFiles.Where(x => x.BatchFileId == batchFile.Id).ToList();
                }

                return returnObj;
            }

            return null;
        }
    }
}
