
using MediatR;

namespace Captive.Applications.Reports.Commands
{
    public class GenerateReportCommand : IRequest<Unit>
    {
        public Guid BatchId {  get; set; }
    }
}
