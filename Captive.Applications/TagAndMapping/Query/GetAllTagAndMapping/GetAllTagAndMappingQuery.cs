using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMapping
{
    public class GetAllTagAndMappingQuery : IRequest<IEnumerable<TagDto>>
    {
        public Guid BankId { get; set; }
    }
}
