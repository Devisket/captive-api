using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMappingByTagId
{
    class GetTagMappingByTagIdQueryHandler : IRequestHandler<GetTagMappingByTagIdQuery, IEnumerable<TagMappingDto>>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetTagMappingByTagIdQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<IEnumerable<TagMappingDto>> Handle(GetTagMappingByTagIdQuery request, CancellationToken cancellationToken)
        {
            var tagMapping = await _readUow.TagsMapping.GetAll()
                .AsNoTracking()
                .Where(x => x.TagId == request.TagId)
                .Select(x => TagMappingDto.ToDto(x))
                .ToListAsync(cancellationToken);

            if (tagMapping == null)
                return new List<TagMappingDto>();

            return tagMapping;
        }
    }
}
