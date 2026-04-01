using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinTable(string tableName)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                tableName
            );
        }
    }
}