
using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckValidation.Query
{
    public class GetCheckValidationQuery : IRequest<GetCheckValidationQueryResponse>
    {
        public Guid Id;
        public Guid BankInfoId { get; set; }
    }
    public class GetCheckValidationQueryResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ValidationType { get; set; }
        public ICollection<TagDto>? Tags { get; set; }
    }
}
