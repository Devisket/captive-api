using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.TagAndMapping.Command.DeleteMapping
{
    public  class DeleteMappingCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
