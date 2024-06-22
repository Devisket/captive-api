using MediatR;
using Microsoft.AspNetCore.Http;

namespace Captive.Applications.OrderFile.Commands.UploadOrderFile
{
    public class UploadOrderFileCommand:IRequest<Unit>
    {
        public required Guid BankId { get; set; }
        public required IEnumerable<IFormFile> Files { get; set; }
    }
}
