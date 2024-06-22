using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.FormsChecks.Command.DeleteFormCheck
{
    public class DeleteFormCheckCommand : IRequest<Unit>
    {
        public Guid BankId { get; set; }
        public Guid FormCheckId { get; set; }
    }
}
