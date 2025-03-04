using Captive.Model.Enums;
using MediatR;

namespace Captive.Applications.Orderfiles.Command.ValidateOrderFile
{
    public class ValidateOrderFileCommand : IRequest<ValidateOrderFileCommandResponse>
    {
        public Guid OrderFileId { get; set; }
    }

    public class ValidateOrderFileCommandResponse
    {
        public LogType LogType { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
