using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Captive.Applications.Batch.Query.GetBatchJobStatus
{
    public class GetBatchJobStatusQueryHandler : IRequestHandler<GetBatchJobStatusQuery, GetBatchJobStatusQueryResponse>
    {
        private readonly IReadUnitOfWork _readUow;

        public GetBatchJobStatusQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<GetBatchJobStatusQueryResponse> Handle(GetBatchJobStatusQuery request, CancellationToken cancellationToken)
        {
            var job = await _readUow.BatchJobs.GetAll()
                .AsNoTracking()
                .Where(x => x.BatchId == request.BatchId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (job == null)
                return new GetBatchJobStatusQueryResponse();

            return new GetBatchJobStatusQueryResponse
            {
                JobId = job.Id,
                Status = job.Status.ToString(),
                Progress = job.Progress,
                CurrentStep = job.CurrentStep,
                Warnings = job.Warnings != null
                    ? JsonConvert.DeserializeObject<List<string>>(job.Warnings) ?? new List<string>()
                    : new List<string>(),
                ErrorMessage = job.ErrorMessage,
            };
        }
    }
}
