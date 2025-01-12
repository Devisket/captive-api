﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Orderfiles.Command.DeleteOrderFile
{
    public class DeleteOrderFileCommand : IRequest<Unit>
    {
        public Guid OrderFileId { get; set; }
    }
}
