using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Command.CreateTag { 
    public class CreateTagMappingCommand : IRequest<Unit>
    {
        public Guid? Id { get; set; }
        public Guid CheckValidationId { get; set; }
        public string TagName { get; set; }
    }
}
