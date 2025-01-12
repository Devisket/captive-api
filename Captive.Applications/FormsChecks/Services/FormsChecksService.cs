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
        public async Task<Data.Models.FormChecks?> GetCheckOrderFormCheck(Guid ProductID, string formType, string checkType, CancellationToken cancellationToken)
        {
            var formCheck = await _readUow.FormChecks.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.CheckType == checkType && x.FormType == formType && x.ProductId == ProductID, cancellationToken);

            return formCheck;
        }
    }
}
