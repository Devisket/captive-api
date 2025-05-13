using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto.ValuesDto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Values
{
    public class ValuesQueryHandler : IRequestHandler<ValuesQuery, ValuesQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public ValuesQueryHandler(IReadUnitOfWork readUow) {
            _readUow = readUow;
        }
        public async Task<ValuesQueryResponse> Handle(ValuesQuery request, CancellationToken cancellationToken)
        {
            var branchList = await _readUow.BankBranches.GetAll()
                .AsNoTracking()
                .Where(x => x.BankInfoId == request.BankId)
                .Select(x => new ValuesDto
                {
                    Id = x.Id,
                    Value = x.BranchName,
                })
                .OrderBy(x => x.Value)
                .ToListAsync();

            var productList = await _readUow.Products.GetAll()
                .AsNoTracking()
                .Where(x => x.BankInfoId == request.BankId)
                .Select(x => new ValuesDto
                {
                    Id = x.Id,
                    Value = x.ProductName,
                })
                .ToListAsync(cancellationToken);

            var formChecks = await _readUow.FormChecks.GetAll().AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Product.BankInfoId == request.BankId)
                .Select(x => new ValuesDto
                {
                    Id = x.Id,
                    Value = String.Format("{0} {1}-{2}", x.FormCheckType.ToString(), x.FormType, x.CheckType),
                    ProductId = x.ProductId
                })
                .ToListAsync(cancellationToken);

            return new ValuesQueryResponse
            {
                BranchValues = branchList!,
                FormCheckValues = formChecks,
                ProductValues = productList!,
            };
        }
    }
}
