using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Captive.Applications.Bank.Query.GetAllBankInfos.Model;

namespace Captive.Applications.Bank.Query.GetAllBankInfos
{
    public class GetAllBankInfoQueryHandler : IRequestHandler<GetAllBankInfoQuery, GetAllBankInfoResponse>
    {

        public IReadUnitOfWork _readUow;
        
        public GetAllBankInfoQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetAllBankInfoResponse> Handle(GetAllBankInfoQuery request, CancellationToken cancellationToken)
        {

            var bankInfos = await _readUow.Banks.GetAll()
                .Select(x => new BankInfo
                {
                    Id = x.Id,
                    BankName = x.BankName,
                    BankDescription = x.BankDescription ?? string.Empty,
                    BankShortName = x.ShortName,
                    CreatedDate = x.CreatedDate
                }).AsNoTracking().ToListAsync(cancellationToken);

            return new GetAllBankInfoResponse
            {
                BankInfos = bankInfos
            };
        }
    }
}
