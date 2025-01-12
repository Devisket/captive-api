using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckValidation.Query.GetCheckValidationById
{
    public class GetCheckValidationByIdQueryHandler : IRequestHandler<GetCheckValidationByIdQuery, GetCheckValidationByIdQueryResponse>
    {
        private readonly IReadUnitOfWork _readUnitOfWork;
        public GetCheckValidationByIdQueryHandler(IReadUnitOfWork readUnitOfWork)
        {
            _readUnitOfWork = readUnitOfWork;
        }

        public async Task<GetCheckValidationByIdQueryResponse> Handle(GetCheckValidationByIdQuery request, CancellationToken cancellationToken)
        {
            var checkValidation = await _readUnitOfWork.CheckValidations.GetAll().Where(x => x.Id == request.Id && x.BankInfoId == request.BankInfoId).Select(x => new GetCheckValidationByIdQueryResponse
            {
                Id = x.Id,
                Name = x.Name,
                Tags = x.Tags.Select(y => new Model.Dto.TagDto
                {
                    Id = y.Id,
                    Name = y.TagName,
                    Mapping = y.Mapping.Select(z => new Model.Dto.TagMappingDto
                    {
                        Id = z.Id,
                        BranchId = z.BranchId,
                        FormCheckId = z.FormCheckId,
                        ProductId = z.ProductId

                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);

            if (checkValidation == null)
            {
                throw new Exception($"Check validation ID{request.Id} doesn't exist");
            }

            return checkValidation;
        }
    }
}
