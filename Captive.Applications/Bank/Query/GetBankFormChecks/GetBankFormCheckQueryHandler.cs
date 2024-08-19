using Captive.Applications.Bank.Query.GetBankFormChecks.Models;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Query.GetBankFormChecks
{
    public class GetBankFormCheckQueryHandler : IRequestHandler<GetBankFormCheckQuery, GetBankFormCheckQueryResponse>
    {

        public readonly IReadUnitOfWork _readUow;

        public GetBankFormCheckQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetBankFormCheckQueryResponse> Handle(GetBankFormCheckQuery request, CancellationToken cancellationToken)
        {
            var bankFormChecks = _readUow.FormChecks.GetAll()
                .Where(x => x.ProductId == request.ProductId).AsNoTracking();


            if(request.Id.HasValue)
                bankFormChecks = bankFormChecks.Where(x => x.Id == request.Id.Value);

            return new GetBankFormCheckQueryResponse
            {
                BankFormChecks = await bankFormChecks.Select(x => new BankFormCheck
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    CheckType = x.CheckType,
                    FormType = x.FormType,
                    Description = x.Description,
                    Quanitity = x.Quantity
                }).ToListAsync(cancellationToken)
            };
        }
    }
}
