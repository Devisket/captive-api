using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Command.DeleteFloatingCheckOrder
{
    public class DeleteFloatingCheckOrderCommand : IRequest<Unit>
    {
        public Guid FloatingCheckOrderId { get; set; }
    }
}
