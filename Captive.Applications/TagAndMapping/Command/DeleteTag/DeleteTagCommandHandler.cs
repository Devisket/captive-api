using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.DeleteTag
{
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public DeleteTagCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id);


            if (tag == null) 
            {
                throw new Exception($"Tag ID: {request.Id} doesn't exist");
            }

            _writeUow.Tags.Delete(tag);

            return Unit.Value;
        }
    }
}
