// ReSharper disable UnusedMember.Global

using Microsoft.AspNetCore.SignalR;

namespace AspireApp.ApiService;

public class WebSocketHub(IServiceProvider serviceProvider) : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        /*
        if (groupName == "Positions")
        {
            using var scope = serviceProvider.CreateScope();
            var positionUpdateService = scope.ServiceProvider.GetRequiredService<PositionUpdateService>();
            BackgroundJob.Enqueue(() => positionUpdateService.ExecuteAsync());
        }
    */
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        /*
        if (groupName == "Positions")
        {
            PositionUpdateService.StopService();
        }
    */
    }
}