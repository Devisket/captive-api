using Captive.Model.Dto;

namespace Captive.Applications.CheckOrder.Services
{
    public interface ICheckOrderService
    {
        void UpdateCheckOrder(CheckOrderDto checkOrderDto);
    }
}
