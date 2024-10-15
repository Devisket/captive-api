using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckValidation.Query.GetCheckValidationById
{
    public class GetCheckValidationByIdQuery : IRequest<GetCheckValidationByIdQueryResponse>
    {
        public Guid Id { get; set; }
        public Guid BankInfoId { get; set; }
    }
    public class GetCheckValidationByIdQueryResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ValidationType { get; set; }
        public ICollection<TagDto>? Tags { get; set; }
    }
}
