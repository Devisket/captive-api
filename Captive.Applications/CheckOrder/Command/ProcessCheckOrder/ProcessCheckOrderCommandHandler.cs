using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckOrder.Command.ProcessCheckOrder
{
    public class ProcessCheckOrderCommandHandler : IRequestHandler<ProcessCheckOrderCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public ProcessCheckOrderCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(ProcessCheckOrderCommand request, CancellationToken cancellationToken)
        {
            /**
             * STEP TO PROCESS
             * 1. Apply check inventory into floating order files
             * 2. Create record out of Check Order table
             * 3. Generate Report
             */

            return Unit.Value;

        }
    }
}
