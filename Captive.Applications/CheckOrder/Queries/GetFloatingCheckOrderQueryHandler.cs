using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Queries
{
    public class GetFloatingCheckOrderQueryHandler : IRequestHandler<GetFloatingCheckOrderQuery, IList<CheckOrderDto>>
    {
        private readonly IReadUnitOfWork _readUow; 

        public GetFloatingCheckOrderQueryHandler(IReadUnitOfWork readUow) { 
            _readUow = readUow;
        }
        public async Task<IList<CheckOrderDto>> Handle(GetFloatingCheckOrderQuery request, CancellationToken cancellationToken)
        {
            var checkDtos = await _readUow.FloatingCheckOrders.GetAll().AsNoTracking().Select(x => new CheckOrderDto
            {
                Id = x.Id,
                AccountNumber = x.AccountNo,
                BRSTN = x.BRSTN,
                CheckType = x.CheckType,
                FormType = x.FormType,
                AccountName1 = x.AccountName1,
                AccountName2 = x.AccountName2,
                Concode = x.Concode,
                DeliverTo = x.DeliverTo,
                BranchCode = x.BranchCode,
                StartingSeries = x.PreStartingSeries,
                EndingSeries = x.PreEndingSeries,
                ErrorMessage = x.ErrorMessage,
                IsOnHold = x.IsOnHold,
                IsValid = x.IsValid,
                MainAccountName = x.AccountName,
                Quantity = x.Quantity,
                
            }).ToListAsync(cancellationToken);

            if (checkDtos == null || !checkDtos.Any()) 
            { 
                return new List<CheckOrderDto>();   
            }

            return checkDtos;
        }
    }
}
