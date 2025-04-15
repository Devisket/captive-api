using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.FormsChecks.Queries.GetAllFormChecks
{
    public class GetAllFormChecksQuery : IRequest<IEnumerable<FormCheckDto>>
    {
        public Guid ProductId { get; set; }
    }
}
