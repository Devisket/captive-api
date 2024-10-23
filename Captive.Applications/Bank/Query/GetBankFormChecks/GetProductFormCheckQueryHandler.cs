using Captive.Applications.Bank.Query.GetBankFormChecks.Models;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Query.GetBankFormChecks
{
    public class GetProductFormCheckQueryHandler : IRequestHandler<GetProductFormCheckQuery, GetProductFormCheckQueryResponse>
    {

        public readonly IReadUnitOfWork _readUow;

        public GetProductFormCheckQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetProductFormCheckQueryResponse> Handle(GetProductFormCheckQuery request, CancellationToken cancellationToken)
        {
            var bankFormChecks = _readUow.FormChecks.GetAll()
                .Where(x => x.ProductId == request.ProductId).AsNoTracking();

            var formChecks = await bankFormChecks.Select(x => new BankFormCheck
            {
                Id = x.Id,
                ProductId = x.ProductId,
                CheckType = x.CheckType,
                FormType = x.FormType,
                Description = x.Description,
                Quanitity = x.Quantity,
                FileInitial = x.FileInitial
            }).ToListAsync(cancellationToken);

            return new GetProductFormCheckQueryResponse
            {
                FormChecks = formChecks
            };
        }
    }
}
