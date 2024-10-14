using MediatR;

namespace Captive.Applications.TagAndMapping.Command.DeleteTag
{
    public class DeleteTagCommand:IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
