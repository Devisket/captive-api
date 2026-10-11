
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;

public interface IProductService
{
    Task<ProductConfiguration> GetProductConfigurationByFileName (string fileName, CancellationToken cancellationToken);
}

public class ProductService : IProductService
{
    private readonly IReadUnitOfWork _readUow;

    public ProductService(IReadUnitOfWork readUow)
    {
        this._readUow = readUow;
    }

    public async Task<ProductConfiguration> GetProductConfigurationByFileName(string fileName, CancellationToken cancellationToken)
    {
         var productConfiguration = await _readUow.ProductConfigurations.GetAll().Include(x => x.Product).FirstOrDefaultAsync(x => fileName.Contains(x.FileName), cancellationToken);
            if (productConfiguration == null)
                throw new Exception($"File name {fileName} has no product configuration");

            return productConfiguration;
    }
}