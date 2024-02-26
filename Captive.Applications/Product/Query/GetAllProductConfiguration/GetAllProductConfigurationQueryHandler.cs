﻿

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
            var response = new GetAllProductConfigurationQueryResponse(request.BankId);

            var productConfigurations = _readUow.ProductConfigurations
                .GetAll()
                .Include(x => x.ProductType)
                .Include(x => x.OrderFileConfiguration)
                .AsNoTracking()
                .Where(x => x.OrderFileConfiguration.BankId == request.BankId);

            var productConfigDtos = await productConfigurations.Select(x => new ProductConfigurationResponse
            {
                Id = x.Id,
                ConfigurationId = x.OrderFileConfigurationId,
                ConfigurationName = x.OrderFileConfiguration.Name,
                ProductTypeId = x.ProductType.Id,
                ProductTypeName= x.ProductType.ProductName
            }).ToListAsync(cancellationToken);

            if (productConfigDtos != null && productConfigDtos.Any())
                response.ProductConfigurations = productConfigDtos;

            return response;          
        }
    }
}
