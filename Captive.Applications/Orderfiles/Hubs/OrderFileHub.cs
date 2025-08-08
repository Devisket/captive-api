using Captive.Model.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Captive.Applications.Orderfiles.Hubs
{
    public class OrderFileHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            //GetConnectionId();
            return base.OnConnectedAsync();
        }

        public async void NotifyDuplicatedCheckOrder(string connectionId, DuplicateCheckOrderDto duplicatedCheckOrder) 
            => await Clients.Client(connectionId).SendAsync("checkDuplicate", duplicatedCheckOrder);

        public Task<string> GetConnectionId() => Task.Run(()=> Context.ConnectionId);

        public async Task JoinBatchGroup(string batchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"batch-{batchId}");
        }

        public async Task LeaveBatchGroup(string batchId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"batch-{batchId}");
        }

        public async Task NotifyOrderFileStatusUpdate(string batchId, OrderfileDto orderFile)
        {
            await Clients.Group($"batch-{batchId}").SendAsync("orderFileStatusUpdate", orderFile);
        }

        public async Task TestConnection(string message)
        {
            await Clients.All.SendAsync("testMessage", $"Test message received: {message}");
        }
    }
}
