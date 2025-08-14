using Captive.Applications.Util;
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
        public IBranchService _branchService;

        public GetBatchQueryByIdHandler(IReadUnitOfWork readUow, IBranchService branchService)
        {
            _readUow = readUow;
            _branchService = branchService;
        }

        public async Task<GetBatchByIdQueryResponse> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
        {
            var batch = await _readUow.BatchFiles.GetAll()
                .Include(x => x.OrderFiles!)
                    .ThenInclude(x => x.FloatingCheckOrders)
                .FirstOrDefaultAsync(x => x.BankInfoId == request.BankId && x.Id == request.BatchId, cancellationToken);

            if (batch == null)
                throw new CaptiveException($"Batch ID: {request.BatchId} doesn't exist.");

            var response = new GetBatchByIdQueryResponse
            {
                Id = batch.Id,
                BatchFileStatus = batch.BatchFileStatus,
                BatchName = batch.BatchName,
                CreatedDate = batch.CreatedDate,
                OrderNumber = batch.OrderNumber,
                OrderFiles = batch.OrderFiles != null && batch.OrderFiles.Any() ? batch.OrderFiles.Select(x => new OrderfileDto
                {
                    Id = x.Id,
                    BatchId = x.BatchFileId,
                    FileName = x.FileName,
                    FileType = Path.GetExtension(x.FileName).SanitizeFileName(),
                    FilePath = x.FilePath,
                    Status = x.Status.ToString(),
                    IsValidated = x.IsValidated,
                    PersonalQuantity = x.PersonalQuantity,
                    CommercialQuantity = x.CommercialQuantity,
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
                        ErrorMessage = c.ErrorMessage,
                        IsOnHold = c.IsOnHold,
                        IsValid = c.IsValid
                    }).ToList() : null
                }).ToList() : null
            };

            foreach (var orderFile in batch!.OrderFiles)
            {
                if (orderFile.CheckOrders == null)
                    continue;
                var checkOrders = orderFile.CheckOrders!.Where(x => string.IsNullOrEmpty(x.DeliverTo));

                foreach (var checkOrder in orderFile.CheckOrders!)
                {
                    if(!String.IsNullOrEmpty(checkOrder.DeliverTo))
                        checkOrder.DeliverTo = await _branchService.GetBranchName(request.BankId, checkOrder.DeliverTo, cancellationToken);
                }
            }

            return response;
        }
    }
}
