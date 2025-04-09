
using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.FormChecks.Command.CreateUpdateFormCheck
{
    public class CreateUpdateFormCheckCommand : IRequest<FormCheckDto>
    {  
        public Guid ProductId { get; set; }        
        public required CreateUpdateFormCheckCommandRequest Detail { get; set; }
    }
}
