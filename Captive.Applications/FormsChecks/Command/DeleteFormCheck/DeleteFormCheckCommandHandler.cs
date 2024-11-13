using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.FormsChecks.Command.DeleteFormCheck
{
    internal class DeleteFormCheckCommandHandler : IRequestHandler<DeleteFormCheckCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public DeleteFormCheckCommandHandler(
            IReadUnitOfWork readUow, 
            IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;

        }

        public async Task<Unit> Handle(DeleteFormCheckCommand request, CancellationToken cancellationToken)
        {
            var formCheck = await _readUow.FormChecks.GetAll().FirstOrDefaultAsync(x => x.Id == request.FormCheckId);

            if (formCheck == null)
                throw new Exception($"FormCheckId:{request.FormCheckId} doesn't exist.");

            _writeUow.FormChecks.Delete(formCheck);

            return Unit.Value;
        }
    }
}
