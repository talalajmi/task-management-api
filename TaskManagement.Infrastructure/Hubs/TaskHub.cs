using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.API.Hubs;

// [Authorize] means only authenticated users can connect
// The JWT token is validated before the connection is established
[Authorize]
public class TaskHub : Hub
{
    // Called when a client connects and joins a project group
    // Clients call this method after connecting to subscribe
    // to updates for a specific project
    public async Task JoinProjectGroup(string projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project:{projectId}");
    }

    // Called when a client wants to stop receiving updates
    // for a specific project
    public async Task LeaveProjectGroup(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project:{projectId}");
    }

    // Called automatically by SignalR when client disconnects
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // SignalR automatically removes the connection from all groups
        // when it disconnects — no manual cleanup needed
        await base.OnDisconnectedAsync(exception);
    }
}
