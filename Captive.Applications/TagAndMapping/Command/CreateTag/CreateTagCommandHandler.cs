using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.TagAndMapping.Command.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateTagCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.HasValue)
            {
                await _writeUow.Tags.AddAsync(new Data.Models.Tag()
                {
                    Id = Guid.NewGuid(),
                    BankId = request.BankId,
                    TagName = request.TagName,
                    SearchByAccount = request.SearchByAccount,
                    SearchByBranch = request.SearchByBranch,
                    SearchByFormCheck = request.SearchByFormCheck,
                    SearchByProduct = request.SearchByProduct,
                    isDefaultTag = request.isDefaultTag,
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
                tag.SearchByProduct = request.SearchByProduct;
                tag.SearchByBranch = request.SearchByBranch;
                tag.SearchByFormCheck = request.SearchByFormCheck;
                tag.SearchByAccount = request.SearchByAccount;
                tag.isDefaultTag = request.isDefaultTag;
            }

            return Unit.Value;
        }
    }
}
