using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.FormsChecks.Queries.GetAllFormChecks
{
    public class GetAllFormChecksQueryHandler : IRequestHandler<GetAllFormChecksQuery, IEnumerable<FormCheckDto>>
    {

        private readonly IReadUnitOfWork _readUow;
        public GetAllFormChecksQueryHandler(IReadUnitOfWork readUow) 
        {
            _readUow = readUow;
        }

        public async Task<IEnumerable<FormCheckDto>> Handle(GetAllFormChecksQuery request, CancellationToken cancellationToken)
        {
            var formChecks = await _readUow.FormChecks.GetAll().Include(x => x.Product).Where(x => x.Product.BankInfoId == request.BankId).Select(x => new FormCheckDto
            {
                Id = x.Id,
                CheckType = x.CheckType,
                FileInitial = x.FileInitial,
                FormType = x.FormType,
                FormCheckType = x.FormCheckType.ToString(),
                ProductId = x.ProductId,
                Description = x.Description,
                Quantity = x.Quantity
            }).ToListAsync(cancellationToken);

            if (formChecks == null || !formChecks.Any())
                return new List<FormCheckDto>();

            return formChecks;
        }
    }
}
