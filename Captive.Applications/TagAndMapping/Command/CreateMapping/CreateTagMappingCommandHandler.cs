using Azure;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.CreateMapping
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
            var tag = await _readUow.Tags.GetAll().FirstOrDefaultAsync(x => x.Id == request.TagId, cancellationToken);

            if (tag == null)
                throw new Exception($"The Tag ID: {request.TagId} doesn't exist");

            if (request.Mappings != null && request.Mappings.Any())
            {
                foreach (var mapping in request.Mappings) {

                    if (mapping.Id.HasValue) {

                        var existingTagMapping = await _readUow.TagsMapping.GetAll().FirstOrDefaultAsync(x => x.Id == mapping.Id, cancellationToken);

                        if (existingTagMapping == null)
                            throw new Exception($"Tag Mapping ID: {mapping.Id} doesn't exist.");

                        existingTagMapping.BranchId = mapping.BranchId;
                        existingTagMapping.ProductId = mapping.ProductId;
                        existingTagMapping.FormCheckId = mapping.FormCheckId;

                        _writeUow.TagMappings.Update(existingTagMapping);
                    }
                    else
                    {
                        var tagMapping = new TagMapping
                        {
                            Id = Guid.NewGuid(),
                            TagId = request.TagId,
                            BranchId = mapping.BranchId,
                            FormCheckId = mapping.FormCheckId,
                            ProductId = mapping.ProductId,
                        };

                        await _writeUow.TagMappings.AddAsync(tagMapping, cancellationToken);
                    }
                }
            }

            return Unit.Value;
        }
    }
}
