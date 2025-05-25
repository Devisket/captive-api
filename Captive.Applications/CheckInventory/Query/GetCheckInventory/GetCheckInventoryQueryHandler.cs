using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Captive.Model.Response;
using Captive.Utility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventory
{
    public class GetCheckInventoryQueryHandler : IRequestHandler<GetCheckInventoryQuery, CheckInventoryQueryResponse>
    {

        private readonly IReadUnitOfWork _readUow;
        public GetCheckInventoryQueryHandler(IReadUnitOfWork readUow) {
            _readUow = readUow;
        }
        public async Task<CheckInventoryQueryResponse> Handle(GetCheckInventoryQuery request, CancellationToken cancellationToken)
        {
            var skipRecord = ((request.CurrentPage - 1) * request.PageSize);

            var rawCheckInventories = await _readUow.CheckInventory.GetAll()
                .AsNoTracking()
                .Where(x => x.TagId == request.TagId 
                    && x.isRepeating == request.IsRepeating 
                    && x.IsActive == request.IsActive)
                .Select(x => CheckInventoryDto.ToDto(x))
                .ToListAsync(cancellationToken);

            var checkInventories = rawCheckInventories.AsQueryable();

            if (request.BranchIds != null && request.BranchIds.Any()) {
                checkInventories = checkInventories.Where(x => x.MappingData.BranchIds.ContainsAny(request.BranchIds));
            }

            if (request.ProductIds!= null && request.ProductIds.Any())
            {
                checkInventories = checkInventories.Where(x => x.MappingData.ProductIds.ContainsAny(request.ProductIds));
            }

            if (request.FormCheckType!= null && request.FormCheckType.Any())
            {
                checkInventories = checkInventories.Where(x => x.MappingData.FormCheckType.ContainsAny(request.FormCheckType));
            }

            var totalRecords = checkInventories.Count();

            if (checkInventories == null ||  !checkInventories.Any()) {
                return new CheckInventoryQueryResponse
                {
                    CheckInventories = new List<CheckInventoryDto>(),
                    TotalCount = 0
                };
            }

            var returnDto = checkInventories
                .Skip(skipRecord)
                .Take(request.PageSize)
                .ToList();

            return new CheckInventoryQueryResponse
            {
                CheckInventories = returnDto,
                TotalCount = totalRecords
            };
        }
    }
}
