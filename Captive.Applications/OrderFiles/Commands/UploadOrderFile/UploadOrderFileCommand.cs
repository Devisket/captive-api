using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.OrderFiles.Commands.UploadOrderFile
{
    public class UploadOrderFileCommand:IRequest<Unit>
    {
        public required string FileName { get; set; }
        public required byte[] OrderFile { get; set; }
    }
}
