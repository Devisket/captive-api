using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.FormsChecks.Services
{
    public class FormsChecksService : IFormsChecksService
    {
        private readonly IReadUnitOfWork _readUow;

        public FormsChecksService(IReadUnitOfWork readUow) 
        {
            _readUow = readUow;
        }
        public async Task<Data.Models.FormChecks?> GetCheckOrderFormCheck(CheckOrderDto checkOrder, CancellationToken cancellationToken)
        {
            var formCheck = await _readUow.FormChecks.GetAll().FirstOrDefaultAsync(x => x.CheckType == checkOrder.CheckType && x.FormType == checkOrder.FormType && x.ProductId == checkOrder.ProductId, cancellationToken);

            return formCheck;
        }
    }
}
