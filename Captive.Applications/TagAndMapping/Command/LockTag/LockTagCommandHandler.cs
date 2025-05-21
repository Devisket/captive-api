using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.LockTag
{
    public class LockTagCommandHandler : IRequestHandler<LockTagCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        public LockTagCommandHandler(IReadUnitOfWork  readUow, IWriteUnitOfWork writeUow) 
        { 
            _readUow = readUow;
            _writeUow = writeUow;
        }


        public async Task<Unit> Handle(LockTagCommand request, CancellationToken cancellationToken)
        {
            var tag =  await _readUow.Tags.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (tag == null) 
            {
                throw new CaptiveException($"Tag ID {request.Id} doesn't exist.");
            }

            tag.IsLock = true;

            _writeUow.Tags.Update(tag);

            return Unit.Value;
        }
    }
}
