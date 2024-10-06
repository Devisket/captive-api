using Captive.Applications.Product.Query.GetAllProductType.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Query.GetAllProductType
{
    public class GetAllProductTypeQueryHandler : IRequestHandler<GetAllProductTypeQuery, GetAllProductTypeQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetAllProductTypeQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetAllProductTypeQueryResponse> Handle(GetAllProductTypeQuery request, CancellationToken cancellationToken)
        {
            var products = await _readUow.ProductTypes.GetAll().Where(x => x.BankInfoId == request.BankId)
                .Select(x => new ProductTypeResponse
                {
                    ProductTypeId = x.Id,
                    ProductTypeName = x.ProductName
                }).ToListAsync();

            return new GetAllProductTypeQueryResponse
            {
                BankId = request.BankId,
                ProductTypes = products
            };
        }
    }
}
