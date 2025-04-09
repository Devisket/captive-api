using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventory
{
    public class GetCheckInventoryQueryHandler : IRequestHandler<GetCheckInventoryQuery, IEnumerable<CheckInventoryDto>>
    {

        private readonly IReadUnitOfWork _readUow;
        public GetCheckInventoryQueryHandler(IReadUnitOfWork readUow) {
            _readUow = readUow;
        }
        public async Task<IEnumerable<CheckInventoryDto>> Handle(GetCheckInventoryQuery request, CancellationToken cancellationToken)
        {
            var checkInventories = await _readUow.CheckInventory.GetAll()
                .AsNoTracking()
                .Where(x => x.TagId == request.TagId)
                .ToListAsync(cancellationToken);

            if (checkInventories == null ||  !checkInventories.Any()) {
                return new List<CheckInventoryDto>();
            }

            var returnDto = checkInventories.Select(x => CheckInventoryDto.ToDto(x));

            return returnDto;
        }
    }
}
