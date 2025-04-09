using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Query.GetProductConfiguration
{
    public class GetProductConfigurationQueryHandler : IRequestHandler<GetProductConfigurationQuery, ProductConfigurationDto>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetProductConfigurationQueryHandler(IReadUnitOfWork readUow) 
        { 
            _readUow = readUow;
        }

        public async Task<ProductConfigurationDto> Handle(GetProductConfigurationQuery request, CancellationToken cancellationToken)
        {
            var productConfigurations = _readUow.ProductConfigurations
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ProductId == request.ProductId);

            var productConfig = await productConfigurations.FirstOrDefaultAsync(cancellationToken);

            if (productConfig == null)
                throw new Exception("Product Config doesn't exist");

            var returnObj = ProductConfigurationDto.ToDto(productConfig);

            return returnObj;
        }
    }
}
