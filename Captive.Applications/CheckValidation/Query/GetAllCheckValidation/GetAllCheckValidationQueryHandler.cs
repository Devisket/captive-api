using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckValidation.Query.GetAllCheckValidation
{
    public class GetAllCheckValidationQueryHandler : IRequestHandler<GetAllCheckValidationQuery, GetAllCheckValidationQueryResponse>
    {
        private readonly IReadUnitOfWork _readUnitOfWork;

        public GetAllCheckValidationQueryHandler(IReadUnitOfWork readUnitOfWork)
        {
            _readUnitOfWork = readUnitOfWork;
        }

        public async Task<GetAllCheckValidationQueryResponse> Handle(GetAllCheckValidationQuery request, CancellationToken cancellationToken)
        {
            var checkValidation = await _readUnitOfWork.CheckValidations.GetAll().Where(x => x.BankInfoId == request.BankInfoId).Select(x => new CheckValidationDto
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
            }).ToListAsync();

            if (checkValidation == null)
            {
                throw new Exception($"Empty check validation for this BankID: {request.BankInfoId}");
            }

            return new GetAllCheckValidationQueryResponse
            {
                CheckValidations = checkValidation
            };
        }
    }
}
