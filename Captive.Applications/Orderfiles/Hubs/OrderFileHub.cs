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
    }
}
