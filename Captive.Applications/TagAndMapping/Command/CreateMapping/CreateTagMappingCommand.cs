using Captive.Model.Application;
using MediatR;

namespace Captive.Applications.TagAndMapping.Command.CreateMapping
{
    public class CreateTagMappingCommand : IRequest<Unit>
    {
        public Guid? Id { get; set; }
        public Guid TagId { get; set; }
        public TagMappingData MappingData { get; set; }
    }
}
