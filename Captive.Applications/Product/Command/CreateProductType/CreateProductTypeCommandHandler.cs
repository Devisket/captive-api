using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Command.CreateProductType
{
    internal class CreateProductTypeCommandHandler : IRequestHandler<CreateProductTypeCommand, Unit>
    {

        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;


        public CreateProductTypeCommandHandler(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(CreateProductTypeCommand request, CancellationToken cancellationToken)
        {
            var isBankExist = await _readUow.Banks.GetAll().AsNoTracking().AnyAsync(x => x.Id == request.BankId, cancellationToken);

            if (!isBankExist)
                throw new Exception($"BankID:{request.BankId} doesn't exist");

            if(request.ProductId.HasValue)
            {
                var isNameExist = await _readUow.Products.GetAll().AsNoTracking().AnyAsync(x => x.BankInfoId == request.BankId && x.ProductName.ToLower() == request.ProductName.ToLower() && x.Id != request.ProductId.Value, cancellationToken);

                if (isNameExist)
                    throw new Exception($"Product name:{request.ProductName} is conflicting");

                var productType = await _readUow.Products.GetAll().FirstOrDefaultAsync(x => x.Id == request.ProductId);

                if (productType == null)
                {
                    throw new Exception($"Product type ID:{request.ProductId}  doesn't exist.");
                }

                productType.ProductName = request.ProductName;
                productType.ProductSequence = request.ProductSequence;

                _writeUow.ProductTypes.Update(productType);
            }
            else
            {
                var isNameExist = await _readUow.Products.GetAll().AsNoTracking().AnyAsync(x => x.BankInfoId == request.BankId && x.ProductName.ToLower() == request.ProductName.ToLower(), cancellationToken);

                if (isNameExist)
                    throw new Exception($"Product name:{request.ProductName} is conflicting");


                await _writeUow.ProductTypes.AddAsync(new Data.Models.Product
                {
                    BankInfoId = request.BankId,
                    ProductName = request.ProductName,
                    ProductSequence = request.ProductSequence
                }, cancellationToken);
            }

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
