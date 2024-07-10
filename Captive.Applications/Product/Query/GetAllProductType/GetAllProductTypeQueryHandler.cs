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
            //var bank = await _readUow.Banks.GetAll().Include(x => x.ProductTypes).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            //if (bank == null)
            //    throw new Exception($"Bank id: {request.BankId} doesn't exist");

            //var productTypes = bank.ProductTypes;

            //if (productTypes == null || !productTypes.Any()) 
            //{
            //    return new GetAllProductTypeQueryResponse
            //    {
            //        BankId = bank.Id,
            //        ProductTypes = []
            //    };
            //}

            //return new GetAllProductTypeQueryResponse
            //{
            //    BankId = bank.Id,
            //    ProductTypes = productTypes.Select(x =>
            //    new ProductTypeResponse
            //    {
            //        ProductTypeId = x.Id,
            //        ProductTypeName = x.ProductName
            //    }).ToList()
            //};

            return null;
        }
    }
}
