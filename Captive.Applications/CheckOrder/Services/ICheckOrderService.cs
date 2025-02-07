using Captive.Data.Models;
using Captive.Model.Dto;

namespace Captive.Applications.CheckOrder.Services
{
    public interface ICheckOrderService
    {
        void UpdateCheckOrder(CheckOrderDto checkOrderDto);
        Task<Tuple<FloatingCheckOrder[], int, int>> ValidateCheckOrder(Guid orderFileId, CancellationToken cancellationToken);
        Task CreateCheckOrder(OrderFile orderFile, CancellationToken cancellationToken);
    }
}
