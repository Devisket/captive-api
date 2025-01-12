using MediatR;

namespace Captive.Applications.Orderfiles.Command.ValidateOrderFile
{
    public class ValidateOrderFileCommand : IRequest<Unit>
    {
        public Guid OrderFileId { get; set; }
    }
}
