using Azure;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.CreateMapping
{
    public class CreateMappingCommandHandler : IRequestHandler<CreateMappingCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateMappingCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateMappingCommand request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll().FirstOrDefaultAsync(x => x.Id == request.TagId, cancellationToken);

            if (tag == null)
                throw new Exception($"The Tag ID: {request.TagId} doesn't exist");

            if (request.Mappings.Any())
            {
                var mapping = request.Mappings.Select(x => new TagMapping
                {
                    TagId = tag.Id,
                    BranchId = x.BranchId,
                    FormCheckId = x.FormCheckId,
                    ProductId = x.ProductId,
                }).ToArray();

                if (mapping.Any())
                    await _writeUow.TagMappings.AddRange(mapping, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
