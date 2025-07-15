using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckOrder.Command.UpdateCheckOrderBarcode
{
    public class UpdateCheckOrderBarcodeCommand : IRequest<Unit>
    {

        public Guid BatchId { get; set; }
        public IEnumerable<UpdateCheckOrderBarcodeDto> CheckordersToUpdate { get; set; }
    }
}
