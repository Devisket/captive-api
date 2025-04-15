using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Command.CreateProductConfiguration
{
    public class CreateProductConfigurationCommandHandler : IRequestHandler<CreateProductConfigurationCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        public CreateProductConfigurationCommandHandler(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }   
        public async Task<Unit> Handle(CreateProductConfigurationCommand request, CancellationToken cancellationToken)
        {
            var product = await _readUow.Products.GetAll().FirstOrDefaultAsync(x => x.Id == request.ProductId);

            if (product == null)
                throw new Exception($"Product with Id: {request.ProductId} doesnt exist");

            var isFilenameConfigurationExist = await _readUow.ProductConfigurations.GetAll()
                .AsNoTracking()
                .AnyAsync(x => x.ProductId == request.ProductId 
                && x.FileName.ToLower() == request.FileName.ToLower() 
                && (!request.Id.HasValue || request.Id.Value != x.Id), 
                cancellationToken);

            if (isFilenameConfigurationExist)
                throw new Exception($"Product configuration file name:{request.FileName} is conflicting");

            if (request.Id.HasValue)
            {
                var existingConfiguration = await _readUow.ProductConfigurations.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if(existingConfiguration == null)
                    throw new Exception($"Product configuration Id :{request.Id} doesn't exist");

                existingConfiguration.FileName = request.FileName;
                existingConfiguration.ConfigurationType = request.ConfigurationType;
                existingConfiguration.ConfigurationData = request.ConfigurationData;

                _writeUow.ProductConfigurations.Update(existingConfiguration);

                return Unit.Value;
            }

            var productConfiguration = new ProductConfiguration()
            {
                Id = Guid.NewGuid(),
                FileName = request.FileName,
                ConfigurationData = request.ConfigurationData,
                ConfigurationType = request.ConfigurationType,
                isActive = true,
                ProductId = product.Id,
                Product = product,
            };

            await _writeUow.ProductConfigurations.AddAsync(productConfiguration, cancellationToken);

            return Unit.Value;
        }
    }
}
