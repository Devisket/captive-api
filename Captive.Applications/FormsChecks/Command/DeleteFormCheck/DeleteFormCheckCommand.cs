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
        public int BankId{ get; set; }
        public int FormCheckId { get; set; }
    }
}
