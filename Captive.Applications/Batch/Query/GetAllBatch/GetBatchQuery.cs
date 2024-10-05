using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Batch.Query.GetAllBatch
{
    public class GetBatchQuery : IRequest<GetBatchQueryResponse>
    {
        public Guid BankId { get; set; }
        public Guid? BatchId { get; set; }
    }
    public class GetBatchQueryResponse
    {
        public ICollection<BatchFileDto> BatchFiles { get; set; }
    }

}
