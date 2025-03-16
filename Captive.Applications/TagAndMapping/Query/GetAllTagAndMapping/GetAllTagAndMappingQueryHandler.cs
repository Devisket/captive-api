using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMapping
{
    public class GetAllTagAndMappingQueryHandler : IRequestHandler<GetAllTagAndMappingQuery, IEnumerable<TagDto>>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetAllTagAndMappingQueryHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
        }
        public async Task<IEnumerable<TagDto>> Handle(GetAllTagAndMappingQuery request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll().Include(x => x.Mapping).Where(x => x.BankId == request.BankId).Select(x => new TagDto
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
            }).ToListAsync(cancellationToken);

            if (tag == null || !tag.Any())
                throw new Exception($"Tag doesn't exist under bankID: {request.BankId}.");

            return tag;
        }
    }
}
