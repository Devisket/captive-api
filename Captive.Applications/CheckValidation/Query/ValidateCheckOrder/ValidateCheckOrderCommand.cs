using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckValidation.Query.ValidateCheckOrder
{
    public class ValidateCheckOrderCommand : IRequest<ValidateCheckOrderDto>
    {
        public Guid OrderId { get; set; }
        public CheckOrderDto[] CheckOrder { get; set; }
    }
}
