using Captive.Applications.Batch.Services;
using Microsoft.AspNetCore.SignalR;

namespace Captive.Applications.Batch.Hubs
{
    public class BatchHub:Hub
    {
        private readonly IBatchService _batchService;
        public BatchHub(IBatchService batchService) : base()
        { 
            _batchService = batchService;
        }
        //Invoke whenever there is changes
        public async Task BroadcastBatchData(Guid batchId)
        {
            var batchData = await _batchService.GetBatchDetailById(batchId);

            await Clients.All.SendAsync($"batch:{batchId}", batchData);
        }

        public Task<string> GetConnectionId() => Task.Run(() => Context.ConnectionId);

        public async Task InvokeGetBatch(string batchId) { 
            await BroadcastBatchData(Guid.Parse(batchId));
        }
    }
}
