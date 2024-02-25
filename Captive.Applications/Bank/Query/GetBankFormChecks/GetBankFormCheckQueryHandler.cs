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
            var bankFormChecks = await _readUow.FormChecks.GetAll()
                .Where(x => x.BankId == request.BankId).AsNoTracking().ToListAsync();

            return new GetBankFormCheckQueryResponse { BankFormChecks = bankFormChecks.Select(x => new BankFormCheck
            {
                Id = x.Id,
                CheckType = x.CheckType,
                FormType = x.FormType,
                Description = x.Description,
                Quanitity = x.Quantity
            }).ToList()};
        }
    }
}
