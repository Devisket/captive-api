using Captive.Data.Models;
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

            var bankExist = await _readUow.Banks.GetAll().AnyAsync(x => x.Id == request.BankId, cancellationToken);
            var productTypeExist = await _readUow.ProductTypes.GetAll().AnyAsync(x => x.Id == request.Detail.ProductTypeId, cancellationToken);

            if (!bankExist)
                throw new Exception($"BankId{request.BankId} doesn't exist.");

            if (!productTypeExist)
                throw new Exception($"ProductTypeId{request.Detail.ProductTypeId} doesn't exist");


            if(request.FormCheckId != Guid.Empty)
            {
                var formCheck = await _readUow.FormChecks.GetAll().FirstOrDefaultAsync(x => x.Id == request.FormCheckId && x.BankId == request.BankId,  cancellationToken);

                if(formCheck == null)
                    throw new Exception($"FormCheckId{request.FormCheckId} doesn't exist");

                formCheck.ProductTypeId = request.Detail.ProductTypeId;
                formCheck.CheckType = request.Detail.CheckType;
                formCheck.FormType = request.Detail.FormType;
                formCheck.Description = request.Detail.Description;
                formCheck.Quantity = request .Detail.Quantity;
                formCheck.FileInitial = request.Detail.FileInitial;

                _writeUow.FormChecks.Update(formCheck);
            }
            else
            {

                await _writeUow.FormChecks.AddAsync(new Captive.Data.Models.FormChecks
                {
                    ProductTypeId = request.Detail.ProductTypeId,
                    CheckType = request.Detail.CheckType,
                    FormType = request.Detail.FormType,
                    Description = request.Detail.Description,
                    FileInitial = request.Detail.FileInitial,
                    Quantity = request.Detail.Quantity
                }, cancellationToken);
            }

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}

