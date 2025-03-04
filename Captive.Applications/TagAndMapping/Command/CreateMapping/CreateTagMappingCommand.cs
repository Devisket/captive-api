using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Command.CreateMapping
{
    public class CreateTagMappingCommand : IRequest<Unit>
    {
        public Guid TagId { get; set; }
        public ICollection<TagMappingDto> Mappings{ get; set; }
    }
}
