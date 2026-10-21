using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Orderfiles.Command.CreateCustomOrderFile
{
    /// <summary>
    /// Creates an order file that is not backed by an uploaded file. The check orders are keyed in
    /// by the user and stored as floating check orders, so the order file goes through the same
    /// validate -> process -> generate flow as an uploaded order file.
    /// </summary>
    public class CreateCustomOrderFileCommand : IRequest<CreateCustomOrderFileCommandResponse>
    {
        public Guid BankId { get; set; }
        public Guid BatchId { get; set; }
        public Guid ProductId { get; set; }
        public string? FileName { get; set; }

        /// <summary>
        /// When enabled, check orders with more than 50 booklets are generated in separate report
        /// files (50 booklets per check order per file). Only available for manually encoded order files.
        /// </summary>
        public bool IsSplitGeneration { get; set; }
        public List<CheckOrderDto> CheckOrders { get; set; } = new();
    }

    public class CreateCustomOrderFileCommandResponse
    {
        public Guid OrderFileId { get; set; }
        public Guid BatchId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int CheckOrderCount { get; set; }
    }
}
