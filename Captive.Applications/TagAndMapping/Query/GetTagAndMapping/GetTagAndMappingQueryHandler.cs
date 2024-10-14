using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMapping
{
    public class GetTagAndMappingQueryHandler : IRequestHandler<GetTagAndMappingQuery, TagDto>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetTagAndMappingQueryHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
        }
        public async Task<TagDto> Handle(GetTagAndMappingQuery request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll().Include(x => x.Mapping).FirstOrDefaultAsync(x => x.Id == request.Id);

            if (tag == null)
                throw new Exception($"Tag ID : {request.Id} doesn't exist");

            return new TagDto
            {
                Id = tag.Id,
                Mapping = tag.Mapping.Any() ?  tag.Mapping.Select(x => new TagMappingDto
                {
                    BranchId = x.BranchId,
                    Id = x.Id,
                    FormCheckId = x.FormCheckId,
                    ProductId = x.ProductId,
                }).ToList() : []
            };
        }
    }
}
