using MediatR;

namespace Captive.Applications.Orderfiles.Command.ValidateOrderFile
{
    public class ValidateOrderFileCommand : IRequest<ValidateOrderFileCommandResponse>
    {
        public Guid OrderFileId { get; set; }
    }

    public class ValidateOrderFileCommandResponse
    {
        public int commercialQuantity { get; set; }
        public int personalQuantity { get; set; }
    }
}
