using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Batch.Query.GetAllBatch
{
    public class GetBatchQuery : IRequest<GetBatchQueryResponse>
    {
        public Guid BankId { get; set; }
    }
    public class GetBatchQueryResponse
    {
        public ICollection<BatchFilesDto> BatchFiles { get; set; }
    }

}
