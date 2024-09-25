using Captive.Applications.ProcessOrderFiles.Queries.GetAllOrderFiles.Model;
using MediatR;

namespace Captive.Applications.ProcessOrderFiles.Queries.GetAllOrderFiles
{
    public class GetAllOrderFilesQuery:IRequest<GetAllOrderFilesQueryResponse>
    {
        public Guid BankId { get; set; }
    }
    public class GetAllOrderFilesQueryResponse
    {
        public Guid BankId { get; set; }
        public ICollection<BatchFileDtoResponse>? Batches { get; set; }
    }
}
