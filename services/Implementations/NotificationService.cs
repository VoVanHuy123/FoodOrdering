using FoodOrdering.services.Interfaces;
using FoodOrdering.Hubs;
using Microsoft.AspNetCore.SignalR;
using FoodOrdering.Context;

namespace FoodOrdering.services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotifyHub> _hub;
        private readonly FoodOrderingContext _context;
        public NotificationService(
            FoodOrderingContext context,
            IHubContext<NotifyHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task CreateCallStaffNotify(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);

            await _hub.Clients.All.SendAsync("NotifyReceive", new
            {
                table.Id,
                table.TableNumber,
            });
        }
    }
}
