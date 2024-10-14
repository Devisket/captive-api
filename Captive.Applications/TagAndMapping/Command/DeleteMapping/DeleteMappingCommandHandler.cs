using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.DeleteMapping
{
    public class DeleteMappingCommandHandler : IRequestHandler<DeleteMappingCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        public DeleteMappingCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(DeleteMappingCommand request, CancellationToken cancellationToken)
        {
            var mapping = await _readUow.TagsMapping.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (mapping == null)
                throw new Exception($"Mapping ID {request.Id} doesn't exist");

            _writeUow.TagMappings.Delete(mapping);

            return Unit.Value;
        }
    }
}
