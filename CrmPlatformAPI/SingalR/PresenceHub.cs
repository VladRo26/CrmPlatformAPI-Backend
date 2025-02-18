    using CrmPlatformAPI.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    namespace CrmPlatformAPI.SingalR
    {
    [Authorize]
    public class PresenceHub(PresenceTracker tracker) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Add the user to the tracker
            await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            // Get the full list of online users
            var currentUsers = await tracker.GetOnlineUsers();
            // Broadcast the full list to all clients
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove the user from the tracker
            await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            // Get the full list of online users
            var currentUsers = await tracker.GetOnlineUsers();
            // Broadcast the updated list to all clients
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
