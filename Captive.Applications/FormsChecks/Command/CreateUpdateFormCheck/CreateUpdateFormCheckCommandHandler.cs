using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.FormChecks.Command.CreateUpdateFormCheck
{
    public class CreateUpdateFormCheckCommandHandler : IRequestHandler<CreateUpdateFormCheckCommand, Unit>
    {
        IReadUnitOfWork _readUow;
        IWriteUnitOfWork _writeUow;

        public CreateUpdateFormCheckCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateUpdateFormCheckCommand request, CancellationToken cancellationToken)
        {
            var productTypeExist = await _readUow.ProductTypes.GetAll().AnyAsync(x => x.Id == request.ProductId, cancellationToken);
            
            if (!productTypeExist)
                throw new Exception($"ProductTypeId{request.ProductId} doesn't exist");

            if (request.Detail.Id != Guid.Empty)
            {
                if (await _readUow.FormChecks.GetAll().AsNoTracking().AnyAsync(x => x.ProductId == request.ProductId && x.CheckType == request.Detail.CheckType && x.FormType == request.Detail.FormType && x.Id != request.Detail.Id))
                {
                    throw new Exception($"Check Type: {request.Detail.CheckType} and Form type: {request.Detail.FormType} has already exist for ProductID: {request.ProductId}");
                }

                var formCheck = await _readUow.FormChecks.GetAll().FirstOrDefaultAsync(x => x.Id == request.Detail.Id, cancellationToken);

                if (formCheck == null)
                    throw new Exception($"FormCheckId{request.Detail.Id} doesn't exist");

                formCheck.CheckType = request.Detail.CheckType;
                formCheck.FormType = request.Detail.FormType;
                formCheck.Description = request.Detail.Description;
                formCheck.Quantity = request.Detail.Quantity;
                formCheck.FileInitial = request.Detail.FileInitial ?? string.Empty;

                _writeUow.FormChecks.Update(formCheck);
            }
            else
            {
                if (await _readUow.FormChecks.GetAll().AsNoTracking().AnyAsync(x => x.ProductId == request.ProductId && x.CheckType == request.Detail.CheckType && x.FormType == request.Detail.FormType))
                {
                    throw new Exception($"Check Type: {request.Detail.CheckType} and Form type: {request.Detail.FormType} has already exist for ProductID: {request.ProductId}");
                }

                await _writeUow.FormChecks.AddAsync(new Captive.Data.Models.FormChecks
                {
                    ProductId = request.ProductId,
                    CheckType = request.Detail.CheckType,
                    FormType = request.Detail.FormType,
                    Description = request.Detail.Description,
                    FileInitial = request.Detail.FileInitial ?? string.Empty,
                    Quantity = request.Detail.Quantity
                }, cancellationToken);
            }

            return Unit.Value;
        }
    }
}

