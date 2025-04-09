using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto.ValuesDto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Values
{
    public class ValuesQueryHandler : IRequestHandler<ValuesQuery, ValuesDto>
    {
        private readonly IReadUnitOfWork _readUow;

        public ValuesQueryHandler(IReadUnitOfWork readUow) {
            _readUow = readUow;
        }
        public async Task<ValuesDto> Handle(ValuesQuery request, CancellationToken cancellationToken)
        {
            var branchList = await _readUow.BankBranches.GetAll()
                .AsNoTracking()
                .Where(x => x.BankInfoId == request.BankId)
                .ToDictionaryAsync(x=> x.Id, x => x.BranchName, cancellationToken);

            var productList = await _readUow.Products.GetAll()
                .AsNoTracking()
                .Where(x => x.BankInfoId == request.BankId)
                .ToDictionaryAsync(x => x.Id, x => x.ProductName, cancellationToken);


            var formChecks = await _readUow.FormChecks.GetAll().AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Product.BankInfoId == request.BankId)
                .Select(x => new { x.Id, FormCheckType = x.FormCheckType, x.FormType, x.CheckType })
                .ToDictionaryAsync(x => x.Id, x => string.Format("{0} {1}-{2}", x.FormCheckType.ToString(), x.FormType, x.CheckType));

            return new ValuesDto
            {
                BranchValues = branchList!,
                FormCheckValues = formChecks,
                ProductValues = productList!,
            };
        }
    }
}
