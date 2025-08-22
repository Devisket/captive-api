using Captive.Applications.Product.Query.GetAllProduct.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Query.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductTypeQuery, ICollection<ProductResponse>>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetAllProductQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<ICollection<ProductResponse>> Handle(GetAllProductTypeQuery request, CancellationToken cancellationToken)
        {
            var products = await _readUow.Products.GetAll().Where(x => x.BankInfoId == request.BankId)
                .Select(x => new ProductResponse
                {
                    ProductId = x.Id,
                    ProductName = x.ProductName,
                    ProductSequence = x.ProductSequence,
                    CustomizeFileName = x.CustomizeFileName,
                }).ToListAsync();

            return products;
        }
    }
}
