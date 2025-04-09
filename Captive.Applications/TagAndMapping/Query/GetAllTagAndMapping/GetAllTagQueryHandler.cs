using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMapping
{
    public class GetAllTagQueryHandler : IRequestHandler<GetAllTagQuery, IEnumerable<TagDto>>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetAllTagQueryHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
        }
        public async Task<IEnumerable<TagDto>> Handle(GetAllTagQuery request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll()
                .AsNoTracking()
                .Where(x => x.BankId == request.BankId).Select(x => TagDto.ToDto(x))
                .ToListAsync(cancellationToken);

            if (tag == null || !tag.Any())
                return new List<TagDto>();

            return tag;
        }
    }
}
