using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Product.Command.DeleteProductType
{
    public class DeleteProductTypeCommandHandler : IRequestHandler<DeleteProductTypeCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public DeleteProductTypeCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }
        public async Task<Unit> Handle(DeleteProductTypeCommand request, CancellationToken cancellationToken)
        {
            var isBankExist = await _readUow.Banks.GetAll().AsNoTracking().AnyAsync(x => x.Id == request.BankId);

            if(!isBankExist)
                throw new Exception($"BankId:{request.BankId} doesn't exist");

            var productType = await _readUow.ProductTypes.GetAll().FirstOrDefaultAsync(x => x.BankInfoId == request.BankId && x.Id == request.ProductTypeId);
            
            if(productType == null)
                throw new Exception($"ProductTypeId:{request.ProductTypeId} doesn't exist");

            _writeUow.ProductTypes.Delete(productType);

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
