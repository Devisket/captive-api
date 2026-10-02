using MediatR;

namespace Captive.Applications.Reports.Commands
{
    /// <summary>
    /// Raised by the orchestrator when barcode/report generation fails for a batch, so the order
    /// files stuck in GeneratingReport are moved to Error and the frontend is notified.
    /// </summary>
    public class ReportGenerationFailedCommand : IRequest<Unit>
    {
        public Guid BatchId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
