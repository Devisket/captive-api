using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.CreateTag
{
    public class CreateTagMappingCommandHandler : IRequestHandler<CreateTagMappingCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateTagMappingCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateTagMappingCommand request, CancellationToken cancellationToken)
        {
            var checkValidation = await _readUow.CheckValidations.GetAll().FirstOrDefaultAsync(x => x.Id == request.CheckValidationId, cancellationToken);

            if (checkValidation == null)
            {
                throw new Exception($"Check validation ID {request.CheckValidationId} is not existing");
            }

            if (!request.Id.HasValue)
            {
                await _writeUow.Tags.AddAsync(new Data.Models.Tag()
                {
                    Id = Guid.NewGuid(),
                    CheckValidationId = checkValidation.Id,
                    TagName = request.TagName,
                }, cancellationToken);
            }
            else
            {
                var tag = await _readUow.Tags.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);

                if(tag == null)
                {
                    throw new Exception($"The TagID: {request.Id.Value} doesn't exist");
                }

                tag.TagName = request.TagName;

            }

            return Unit.Value;
        }
    }
}
