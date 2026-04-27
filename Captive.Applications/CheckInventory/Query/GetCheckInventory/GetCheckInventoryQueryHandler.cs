using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Captive.Model.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventory
{
    public class GetCheckInventoryQueryHandler : IRequestHandler<GetCheckInventoryQuery, CheckInventoryQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetCheckInventoryQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<CheckInventoryQueryResponse> Handle(GetCheckInventoryQuery request, CancellationToken cancellationToken)
        {
            var skipRecord = (request.CurrentPage - 1) * request.PageSize;

            var query = _readUow.CheckInventory.GetAll()
                .AsNoTracking()
                .Include(x => x.Mappings)
                .Where(x => x.BankId == request.BankId);

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (request.IsRepeating.HasValue)
                query = query.Where(x => x.isRepeating == request.IsRepeating.Value);

            if (request.BranchIds != null && request.BranchIds.Any())
            {
                var branchIds = request.BranchIds.ToList();
                query = query.Where(x =>
                    !x.Mappings.Any(m => m.BranchId.HasValue) ||
                    x.Mappings.Any(m => m.BranchId.HasValue && branchIds.Contains(m.BranchId!.Value)));
            }

            if (request.ProductIds != null && request.ProductIds.Any())
            {
                var productIds = request.ProductIds.ToList();
                query = query.Where(x =>
                    !x.Mappings.Any(m => m.ProductId.HasValue) ||
                    x.Mappings.Any(m => m.ProductId.HasValue && productIds.Contains(m.ProductId!.Value)));
            }

            if (request.FormCheckType != null && request.FormCheckType.Any())
            {
                var formCheckTypes = request.FormCheckType.ToList();
                query = query.Where(x =>
                    !x.Mappings.Any(m => m.FormCheckType != null) ||
                    x.Mappings.Any(m => m.FormCheckType != null && formCheckTypes.Contains(m.FormCheckType)));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            if (totalRecords == 0)
                return new CheckInventoryQueryResponse { CheckInventories = new List<CheckInventoryDto>(), TotalCount = 0 };

            var entities = await query
                .Skip(skipRecord)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new CheckInventoryQueryResponse
            {
                CheckInventories = entities.Select(CheckInventoryDto.ToDto).ToList(),
                TotalCount = totalRecords
            };
        }
    }
}
