using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Batch.Commands.ValidateBatch
{
    public class ValidateBatchCommand : IRequest<IEnumerable<LogDto>>
    {
        public Guid BatchId { get; set; }
    }
}
