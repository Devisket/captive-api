using MediatR;

namespace Captive.Applications.Batch.Commands.DeleteBatchFile
{
    public class DeleteBatchFileCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
