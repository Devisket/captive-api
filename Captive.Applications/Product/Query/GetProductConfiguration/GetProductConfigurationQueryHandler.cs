using Captive.Applications.Product.Query.GetProductConfiguration.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Query.GetProductConfiguration
{
    public class GetProductConfigurationQueryHandler : IRequestHandler<GetProductConfigurationQuery, GetAllProductConfigurationQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetProductConfigurationQueryHandler(IReadUnitOfWork readUow) 
        { 
            _readUow = readUow;
        }

        public async Task<GetAllProductConfigurationQueryResponse> Handle(GetProductConfigurationQuery request, CancellationToken cancellationToken)
        {
            var response = new GetAllProductConfigurationQueryResponse();

            var productConfigurations = _readUow.ProductConfigurations
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ProductId == request.ProductId);

            var productConfigDto = await productConfigurations.Select(x => new ProductConfigurationResponse
            {
                Id = x.Id,
                ProductId = x.Product.Id,
                FileName = x.FileName,
                ProductName = x.Product.ProductName,
                ConfigurationData = x.ConfigurationData,
                ConfigurationType = x.ConfigurationType
            }).FirstOrDefaultAsync(cancellationToken);

            if (productConfigDto != null)
                response.ProductConfiguration = productConfigDto;

            return response;          
        }
    }
}
