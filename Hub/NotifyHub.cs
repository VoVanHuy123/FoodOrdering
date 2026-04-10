using Microsoft.AspNetCore.SignalR;
namespace FoodOrdering.Hubs
{
    public class NotifyHub : Hub
    {
      
            // User join notification group
            public async Task JoinUser(string userId)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"USER_{userId}"
                );
            }

            // Admin join admin group
            public async Task JoinAdmin()
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    "ADMINS"
                );
            }
    
    }
}
