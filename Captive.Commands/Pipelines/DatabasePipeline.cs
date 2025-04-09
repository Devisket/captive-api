using Azure;
using Captive.Data;
using Captive.Data.UnitOfWork.Write;
using MediatR;

namespace Captive.Commands.Pipelines
{
    public class DatabasePipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly CaptiveDataContext _dbContext;
        public DatabasePipeline(CaptiveDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (_dbContext.ChangeTracker.HasChanges())
            {
                await _dbContext.SaveChangesAsync();
            }

            return response;
        }
    }
}
