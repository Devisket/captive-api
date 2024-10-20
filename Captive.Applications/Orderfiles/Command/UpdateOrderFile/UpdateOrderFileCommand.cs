using Captive.Data.Enums;
using MediatR;

namespace Captive.Applications.Orderfiles.Command.UpdateOrderFile
{
    public class UpdateOrderFileCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string? ErrorMessage {  get; set; }
        public OrderFilesStatus Status { get; set; }
    }
}
