using Captive.Data.Enums;
using MediatR;

namespace Captive.Applications.Batch.Commands.UpdateBatchFileStatus
{
    public class UpdateBatchFileStatusCommand: IRequest<Unit>
    {
        public Guid BatchId { get; set; }   
        public string BatchFileStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}
