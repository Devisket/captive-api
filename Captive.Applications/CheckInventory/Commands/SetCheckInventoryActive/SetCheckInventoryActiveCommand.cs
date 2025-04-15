using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckInventory.Commands.SetCheckInventoryActive
{
    public class SetCheckInventoryActiveCommand : IRequest<Unit>
    {
        public Guid CheckInventoryId { get; set; }
    }
}
