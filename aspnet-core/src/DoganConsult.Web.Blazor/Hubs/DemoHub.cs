using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DoganConsult.Web.Demos;

namespace DoganConsult.Web.Blazor.Hubs
{
    public class DemoHub : Hub
    {
        public async Task NotifyDemoCreatedAsync(DemoRequestDto demo)
        {
            await Clients.All.SendAsync("DemoCreated", demo);
        }

        public async Task NotifyDemoUpdatedAsync(DemoRequestDto demo)
        {
            await Clients.All.SendAsync("DemoUpdated", demo);
        }

        public async Task NotifyDemoApprovedAsync(DemoRequestDto demo)
        {
            await Clients.All.SendAsync("DemoApproved", demo);
        }

        public async Task NotifyDemoRejectedAsync(DemoRequestDto demo)
        {
            await Clients.All.SendAsync("DemoRejected", demo);
        }

        public async Task NotifyDemoStatusChangedAsync(DemoRequestDto demo)
        {
            await Clients.All.SendAsync("DemoStatusChanged", demo);
        }

        public async Task JoinDemoGroup(int demoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"demo_{demoId}");
        }

        public async Task LeaveDemoGroup(int demoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"demo_{demoId}");
        }
    }
}
