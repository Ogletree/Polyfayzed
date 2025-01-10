using Microsoft.AspNetCore.SignalR;
// ReSharper disable UnusedMember.Global

namespace AspireApp.ApiService;

public class WebSocketHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}