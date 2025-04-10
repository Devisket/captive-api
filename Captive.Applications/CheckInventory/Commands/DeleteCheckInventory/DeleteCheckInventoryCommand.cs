using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckInventory.Commands.DeleteCheckInventory
{
    public class DeleteCheckInventoryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
