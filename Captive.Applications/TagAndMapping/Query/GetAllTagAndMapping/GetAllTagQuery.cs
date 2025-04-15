using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMapping
{
    public class GetAllTagQuery : IRequest<IEnumerable<TagDto>>
    {
        public Guid BankId { get; set; }
    }
}
