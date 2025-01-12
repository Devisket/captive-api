using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Command.ProcessCheckOrder
{
    public class ProcessCheckOrderCommand : IRequest<Unit>
    {
       public Guid OrderFileId { get; set; }
    }
}
