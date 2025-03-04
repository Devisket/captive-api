using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMappingByTagId
{
    class GetTagAndMappingByTagIdQueryHandler : IRequestHandler<GetTagAndMappingByTagIdQuery, TagDto>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetTagAndMappingByTagIdQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<TagDto> Handle(GetTagAndMappingByTagIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll().Include(x => x.Mapping).Select(x => new TagDto
            {
                Id = x.Id,
                BankId = x.BankId,
                isDefaultTag = x.isDefaultTag,
                SearchByAccount = x.SearchByAccount,
                SearchByBranch = x.SearchByBranch,
                SearchByFormCheck = x.SearchByFormCheck,
                SearchByProduct = x.SearchByProduct,
                TagName = x.TagName,
                Mapping = x.Mapping.Select(z => new TagMappingDto
                {
                    Id = z.Id,
                    BranchId = z.BranchId,
                    FormCheckId = z.FormCheckId,
                    ProductId = z.ProductId
                }).ToList()
            }).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (tag == null)
                throw new Exception($"TagID : {request.Id} doens't exist.");

            return tag;
        }
    }
}
