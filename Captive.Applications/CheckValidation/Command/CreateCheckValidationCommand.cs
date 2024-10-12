using Captive.Data.Enums;
using MediatR;

namespace Captive.Applications.CheckValidation.Command
{
    public class CreateCheckValidationCommand:IRequest<Unit>
    {
        public Guid? Id { get; set; }
        public Guid BankId { get; set; }    
        public required string Name {  get; set; }
        public required string ValidationType { get; set; }
    }
}
