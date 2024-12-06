using Captive.Data.UnitOfWork.Read;
using Captive.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Reports.Commands
{
    public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, Unit>
    {
        private readonly IReportGenerator _reportGenerator;
        private readonly IReadUnitOfWork _readUow;

        public GenerateReportCommandHandler(IReportGenerator reportGenerator, IReadUnitOfWork readUow) 
        {
            _reportGenerator = reportGenerator;
            _readUow = readUow;
        }
        public async Task<Unit> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
        {

            var batchFile = await _readUow.BatchFiles.GetAll().Include(x => x.OrderFiles).AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.BatchId);


            if (batchFile == null)
                throw new Exception($"BatchID {request.BatchId} doesn't exist");

            if (batchFile.OrderFiles.Any(x => x.Status == Data.Enums.OrderFilesStatus.Error)) { 
            
            }

            await _reportGenerator.OnGenerateReport(request.BatchId, cancellationToken);

            return Unit.Value;
        }
    }
}
