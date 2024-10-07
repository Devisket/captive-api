using Captive.Data.Enums;
using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Batch.Query.GetBatchById
{
    public class GetBatchByIdQuery : IRequest<GetBatchByIdQueryResponse>
    {
        public Guid BankId { get; set; }
        public Guid BatchId { get; set; }
    }
    public class GetBatchByIdQueryResponse
    {
        public Guid Id { get; set; }
        public required int OrderNumber { get; set; }
        public required string BatchName { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required BatchFileStatus BatchFileStatus { get; set; }
        public ICollection<OrderfileDto>? OrderFiles { get; set; }
    }
}
