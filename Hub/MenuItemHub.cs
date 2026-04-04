using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Hubs
{
    public class MenuItemHub : Hub
    {
        // Join group theo MenuItem
        public async Task JoinMenuItem(int menuItemId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"menuitem-{menuItemId}"
            );
        }

        // Leave group
        public async Task LeaveMenuItem(int menuItemId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"menuitem-{menuItemId}"
            );
        }
    }
}