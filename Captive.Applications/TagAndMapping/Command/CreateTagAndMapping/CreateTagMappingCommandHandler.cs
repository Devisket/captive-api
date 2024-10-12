using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.CreateTagAndMapping
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

                var newTag = new Data.Models.Tag()
                {
                    Id = Guid.NewGuid(),
                    CheckValidationId = checkValidation.Id,
                    TagName = request.TagName,
                };

                await _writeUow.Tags.AddAsync(newTag, cancellationToken);

                await _writeUow.Complete();

                if (request.TagMappings.Any())
                {
                    var mapping = request.TagMappings.Select(x => new TagMapping
                    {
                        TagId = newTag.Id,
                        BranchId = x.BranchId,
                        FormCheckId = x.FormCheckId,
                        ProductId = x.ProductId,
                    }).ToArray();

                    if (mapping.Any())
                        await _writeUow.TagMappings.AddRange(mapping, cancellationToken);
                }

            }

            return Unit.Value;
        }
    }
}
