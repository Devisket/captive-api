using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckValidation.Command
{
    public class CreateCheckValidationCommandHandler : IRequestHandler<CreateCheckValidationCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public CreateCheckValidationCommandHandler(IWriteUnitOfWork writeUnitOfWork, IReadUnitOfWork readUnitOfWork) 
        {
            _readUow = readUnitOfWork;
            _writeUow = writeUnitOfWork;
        }
        public async Task<Unit> Handle(CreateCheckValidationCommand request, CancellationToken cancellationToken)
        {
            var validationType = (ValidationType)Enum.Parse(typeof(ValidationType), request.ValidationType);

            if (request.Id.HasValue)
            {
                var checkValidation = await _readUow.CheckValidations.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id && x.BankInfoId == request.BankId);

                if (checkValidation == null) 
                    throw new Exception($"Check ID: {request.Id} doesn't exist.");
                
                checkValidation.Name = request.Name;
                checkValidation.ValidationType = validationType;

                _writeUow.CheckValidations.Update(checkValidation);

                return Unit.Value;
            }

            await _writeUow.CheckValidations.AddAsync(new Data.Models.CheckValidation
            {
                Name = request.Name,
                ValidationType = validationType,
                BankInfoId = request.BankId
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
