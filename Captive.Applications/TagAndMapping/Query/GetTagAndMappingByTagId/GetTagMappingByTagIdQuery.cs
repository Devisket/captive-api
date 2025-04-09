using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMappingByTagId
{
    public class GetTagMappingByTagIdQuery :IRequest<IEnumerable<TagMappingDto>>
    {
        public Guid TagId { get; set; }
    }
}
