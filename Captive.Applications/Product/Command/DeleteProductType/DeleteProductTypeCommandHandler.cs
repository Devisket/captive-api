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
            var product = await _readUow.ProductTypes.GetAll().FirstOrDefaultAsync(x => x.Id == request.ProductId);
            
            if(product == null)
                throw new Exception($"ProductTypeId:{request.ProductId} doesn't exist");

            _writeUow.ProductTypes.Delete(product);

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
