using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Command.CreateTagAndMapping
{
    public class CreateTagMappingCommand : IRequest<Unit>
    {
        public Guid? Id { get; set; }
        public Guid CheckValidationId { get; set; }
        public string TagName { get; set; }
        public ICollection<TagMappingDto> TagMappings { get; set; }
    }
}
