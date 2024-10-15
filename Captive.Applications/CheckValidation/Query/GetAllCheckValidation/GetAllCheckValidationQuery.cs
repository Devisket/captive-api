using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckValidation.Query.GetAllCheckValidation
{
    public class GetAllCheckValidationQuery :IRequest<GetAllCheckValidationQueryResponse>
    {
        public Guid BankInfoId { get; set; }
    }

    public class GetAllCheckValidationQueryResponse
    {
        public ICollection<CheckValidationDto> CheckValidations { get; set; }
    }
}
