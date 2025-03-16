using Captive.Model.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Command.ProcessCheckOrder
{
    public class ProcessCheckOrderCommand : IRequest<ProcessCheckOrderCommandResponse>
    {
       public Guid OrderFileId { get; set; }
    }

    public class ProcessCheckOrderCommandResponse : LogDto
    {

    }
}
