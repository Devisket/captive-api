
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;

namespace Captive.Applications.CheckOrder.Services
{
    public class CheckOrderService : ICheckOrderService
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CheckOrderService(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public void UpdateCheckOrder(CheckOrderDto checkOrderDto)
        {
            _writeUow.CheckOrders.Update(
                new CheckOrders
                {
                    Id = Guid.Empty,
                    AccountNo = checkOrderDto.AccountNumber,
                    AccountName = string.Concat(checkOrderDto.AccountName1, checkOrderDto.AccountName2),
                    BRSTN = checkOrderDto.BRSTN,
                    OrderQuanity = checkOrderDto.Quantity,
                    FormCheckId = null,
                    DeliverTo = checkOrderDto.DeliverTo,
                    Concode = checkOrderDto.Concode,
                }
              ); 
        }
    }
}
