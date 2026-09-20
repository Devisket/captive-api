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
