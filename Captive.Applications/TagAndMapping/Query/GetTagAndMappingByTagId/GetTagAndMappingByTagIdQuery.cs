using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMappingByTagId
{
    public class GetTagAndMappingByTagIdQuery :IRequest<TagDto>
    {
        public Guid Id { get; set; }
    }
}
