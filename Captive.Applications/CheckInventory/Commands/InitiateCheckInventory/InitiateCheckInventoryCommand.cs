using Captive.Model.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckInventory.Commands.InitiateCheckInventory
{
    public  class InitiateCheckInventoryCommand: CheckInventoryDto, IRequest<Unit>
    {
    }
}
