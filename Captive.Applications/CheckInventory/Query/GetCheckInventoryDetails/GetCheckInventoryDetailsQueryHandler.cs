using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Captive.Model.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventoryDetails
{
    public class GetCheckInventoryDetailsQueryHandler : IRequestHandler<GetCheckInventoryDetailsQuery, CheckInventoryDetailsQueryResponse>
    {
        private const int MaxPageSize = 100;

        private readonly IReadUnitOfWork _readUow;

        public GetCheckInventoryDetailsQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<CheckInventoryDetailsQueryResponse> Handle(GetCheckInventoryDetailsQuery request, CancellationToken cancellationToken)
        {
            if (!request.Unassigned && !request.CheckInventoryId.HasValue)
                throw new CaptiveException("Provide a check inventory ID or request the unassigned series.", 400);

            var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
            var currentPage = Math.Max(request.CurrentPage, 1);

            var query = _readUow.CheckInventoryDetails.GetAll().AsNoTracking();

            if (request.Unassigned)
            {
                // Series not taken from a check inventory but tied to a check order of this bank
                query = query.Where(x =>
                    x.CheckInventoryId == null &&
                    x.CheckOrderId != null &&
                    x.CheckOrder!.OrderFile.BatchFile!.BankInfoId == request.BankId);
            }
            else
            {
                var checkInventoryId = request.CheckInventoryId!.Value;
                query = query.Where(x =>
                    x.CheckInventoryId == checkInventoryId &&
                    x.CheckInventory!.BankId == request.BankId);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(x =>
                    (x.StartingSeries != null && x.StartingSeries.Contains(search)) ||
                    (x.EndingSeries != null && x.EndingSeries.Contains(search)) ||
                    (x.AccountNumber != null && x.AccountNumber.Contains(search)) ||
                    (x.CheckOrder != null && (x.CheckOrder.AccountNo.Contains(search) || x.CheckOrder.AccountName.Contains(search))));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0)
                return new CheckInventoryDetailsQueryResponse();

            var details = await query
                .OrderByDescending(x => x.CreatedDateTime)
                .ThenByDescending(x => x.StartingNumber)
                .ThenByDescending(x => x.StartingSeries)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CheckInventoryDetailsDto
                {
                    Id = x.Id,
                    CheckInventoryId = x.CheckInventoryId,
                    CheckOrderId = x.CheckOrderId,
                    StartingSeries = x.StartingSeries,
                    EndingSeries = x.EndingSeries,
                    Quantity = x.Quantity,
                    AccountNumber = x.CheckOrder != null ? x.CheckOrder.AccountNo : x.AccountNumber,
                    CheckOrderName = x.CheckOrder != null ? x.CheckOrder.AccountName : null,
                    OrderFileName = x.CheckOrder != null ? x.CheckOrder.OrderFile.FileName : null,
                    BatchName = x.CheckOrder != null && x.CheckOrder.OrderFile.BatchFile != null
                        ? x.CheckOrder.OrderFile.BatchFile.BatchName
                        : null,
                    CreatedDateTime = x.CreatedDateTime,
                })
                .ToListAsync(cancellationToken);

            return new CheckInventoryDetailsQueryResponse
            {
                Details = details,
                TotalCount = totalCount,
            };
        }
    }
}
