using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckOrder.Queries
{
    public class GetFloatingCheckOrderQuery : IRequest<IList<CheckOrderDto>>
    {
        public Guid OrderFileId { get; set; }
    }
}
