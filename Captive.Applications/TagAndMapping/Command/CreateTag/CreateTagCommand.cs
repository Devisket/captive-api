using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.TagAndMapping.Command.CreateTag { 
    public class CreateTagCommand : IRequest<Unit>
    {
        public Guid? Id { get; set; }
        public Guid BankId { get; set; }
        public required string TagName { get; set; }
        public bool SearchByBranch { get; set; } = false;
        public bool SearchByAccount { get; set; } = false;
        public bool SearchByFormCheck { get; set; } = false;
        public bool SearchByProduct { get; set; } = false;
        public bool isDefaultTag { get; set; } = false;
    }
}
