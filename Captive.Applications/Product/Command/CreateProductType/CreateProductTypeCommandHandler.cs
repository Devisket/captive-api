﻿using Captive.Data.UnitOfWork.Read;
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

            var isNameExist = await _readUow.Products.GetAll().AsNoTracking().AnyAsync(x=> x.BankInfoId == request.BankId && x.ProductName.ToLower() == request.ProductName.ToLower(), cancellationToken);

            if (!isBankExist)
                throw new Exception($"BankID:{request.BankId} doesn't exist");

            if (isNameExist)
                throw new Exception($"Product name:{request.ProductName} is conflicting");

            if(request.ProductId.HasValue)
            {
                var productType = await _readUow.Products.GetAll().FirstOrDefaultAsync(x => x.Id == request.ProductId);

                if (productType == null)
                {
                    throw new Exception($"Product type ID:{request.ProductId}  doesn't exist.");
                }

                productType.ProductName = request.ProductName;

                _writeUow.ProductTypes.Update(productType);
            }
            else
            {
                await _writeUow.ProductTypes.AddAsync(new Data.Models.Product
                {
                    BankInfoId = request.BankId,
                    ProductName = request.ProductName
                }, cancellationToken);
            }

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
