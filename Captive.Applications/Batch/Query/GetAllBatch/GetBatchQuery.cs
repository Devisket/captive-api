using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Batch.Query.GetAllBatch
{
    public class GetBatchQuery : IRequest<ICollection<BatchFilesDto>>
    {
        public Guid BankId { get; set; }
    }
}
