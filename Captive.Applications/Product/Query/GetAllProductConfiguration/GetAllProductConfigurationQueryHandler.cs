using Captive.Applications.Product.Query.GetAllProductConfiguration.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Query.GetAllProductConfiguration
{
    public class GetAllProductConfigurationQueryHandler : IRequestHandler<GetAllProductConfigurationQuery, GetAllProductConfigurationQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetAllProductConfigurationQueryHandler(IReadUnitOfWork readUow) 
        { 
            _readUow = readUow;
        }

        public async Task<GetAllProductConfigurationQueryResponse> Handle(GetAllProductConfigurationQuery request, CancellationToken cancellationToken)
        {
            var response = new GetAllProductConfigurationQueryResponse();

            var productConfigurations = _readUow.ProductConfigurations
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ProductId == request.ProductId);

            var productConfigDtos = await productConfigurations.Select(x => new ProductConfigurationResponse
            {
                Id = x.Id,
                ProductId = x.Product.Id,
                ProductName = x.Product.ProductName,
                ConfigurationData = x.ConfigurationData,
            }).ToListAsync(cancellationToken);

            if (productConfigDtos != null && productConfigDtos.Any())
                response.ProductConfigurations = productConfigDtos;

            return response;          
        }
    }
}
