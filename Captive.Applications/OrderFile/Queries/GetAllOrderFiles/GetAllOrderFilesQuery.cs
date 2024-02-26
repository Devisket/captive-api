using Captive.Applications.OrderFile.Queries.GetAllOrderFiles.Model;
using MediatR;

namespace Captive.Applications.OrderFile.Queries.GetAllOrderFiles
{
    public class GetAllOrderFilesQuery:IRequest<GetAllOrderFilesQueryResponse>
    {
        public int BankId { get; set; }
    }
    public class GetAllOrderFilesQueryResponse
    {
        public int BankId { get; set; }
        public ICollection<BatchFileDtoResponse>? Batches { get; set; }
    }
}
