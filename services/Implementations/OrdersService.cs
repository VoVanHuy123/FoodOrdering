using CloudinaryDotNet.Actions;
using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Hubs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.services.Implementations
{
    public class OrdersService : IOrdersService
    {
        private readonly FoodOrderingContext _context;
        private readonly IHubContext<OrderHub> _hub;

        public OrdersService(
            FoodOrderingContext context,
            IHubContext<OrderHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Table) 

                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            order.Status = newStatus;
            order.UpdateTime = DateTime.Now;

            if (newStatus == "Completed" || newStatus == "Canceled")
            {
                order.Table?.Status = "Available";
            }

            await _context.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("OrderUpdated", new
            {
                id = order.Id,
                status = order.Status
            });

            if (order.Table != null)
            {
                await _hub.Clients.All.SendAsync("TableOccupied", new
                {
                    id = order.Table.Id,
                    tableNumber = order.Table.TableNumber,
                    status = order.Table.Status
                });
            }

            return true;
        }

        // ================= GET ALL =================
        public async Task<PagedResult<OrderDTO>> GetAllAsync(OrderQuery query)
        {
            var orders = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.MenuItem)
                .AsQueryable();

            // ===== FILTER TABLE =====
            if (query.TableNumber.HasValue)
            {
                orders = orders.Where(o =>
                    o.Table.TableNumber == query.TableNumber.Value);
            }

            // ===== FILTER STATUS =====
            if (!string.IsNullOrEmpty(query.Status))
            {
                orders = orders.Where(o => o.Status == query.Status);
            }

            // ===== TOTAL COUNT =====
            var totalItems = await orders.CountAsync();

            // ===== PAGINATION =====
            var data = await orders
                //.OrderByDescending(o => o.OrderTime)
                .OrderBy(o =>
        o.Status == "Preparing" ? 1 :
        o.Status == "Pending" ? 2 :
        o.Status == "Completed" ? 3 :
        o.Status == "Cancelled" ? 4 : 5)
                .ThenByDescending(o => o.OrderTime)
                .ThenByDescending(o => o.UpdateTime)    // 2. UpdateTime

                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    TableId = o.TableId,
                    OrderTime = o.OrderTime,
                    IsError = o.IsError,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    Note = o.Note
                })
                .ToListAsync();

            return new PagedResult<OrderDTO>
            {
                TotalItems = totalItems,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Items = data
            };
        }

        // ================= GET BY ID =================
        public async Task<OrderDTO?> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderDTO
            {
                Id = order.Id,
                TableId = order.TableId,
                OrderTime = order.OrderTime,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Note = order.Note,
                Items = order.OrderItems.Select(i => new OrderItemDTO
                {
                    MenuItemId = i.MenuItemId,
                    IsAvailable = i.MenuItem.IsAvailable,
                    MenuItemName = i.MenuItem != null ? i.MenuItem.Name : null,
                    ImageUrl = i.MenuItem.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Note = i.Note
                }).ToList()
            };
        }
        //================== GET BY MENU ITEM ID =================
        public async Task UpdateOrdersUpdateTimeByMenuItemNotAvailableAsync(int menuItemId, bool isErrorUpdate)
        {
            List<int> orderIds = new List<int>();
            var orders = await _context.Orders
                .Where(o =>
                    (o.Status == "Pending" || o.Status == "Preparing" && o.IsError == !isErrorUpdate) &&
                    o.OrderItems.Any(i => i.MenuItemId == menuItemId)
                )
                .ToListAsync();

            foreach (var order in orders)
            {
                orderIds.Add(order.Id);
                order.UpdateTime = DateTime.Now;
                order.IsError = isErrorUpdate;
            }

            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("MenuItemNotAvailable", new
            {
                orderIds
            });
        }

        // ================= CREATE =================
        public async Task<OrderDTO> CreateAsync(OrderDTO dto)
        {
            var tableExists = await _context.Tables.AnyAsync(t => t.Id == dto.TableId);
            if (!tableExists)
            {
                throw new Exception("TableId không tồn tại");
            }
            var table = await _context.Tables.FindAsync(dto.TableId);
            table.Status = "Occupied";
            await _context.SaveChangesAsync();
            var order = new Orders
            {
                TableId = dto.TableId,
                OrderTime = DateTime.Now,
                Status = dto.Status,
                Note = dto.Note,
                OrderItems = dto.Items?.Select(i => new OrderItems
                {
                    MenuItemId = i.MenuItemId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Note = i.Note
                }).ToList() ?? new List<OrderItems>()
            };

            order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Recalculate total
            order.TotalAmount = await _context.OrderItems
                .Where(i => i.OrderId == order.Id)
                .SumAsync(i => i.Quantity * i.Price);

            // CHECK OUT OF STOCK
            var menuItemIds = await _context.OrderItems
                .Where(i => i.OrderId == order.Id)
                .Select(i => i.MenuItemId)
                .ToListAsync();

            order.IsError = await _context.MenuItems
                .AnyAsync(m => menuItemIds.Contains(m.Id) && !m.IsAvailable);

            await _context.SaveChangesAsync();

            dto.Id = order.Id;
            dto.OrderTime = order.OrderTime;
            dto.TotalAmount = order.TotalAmount;
            dto.IsError = order.IsError;


            // SIGNALR REALTIME
            await _hub.Clients.All.SendAsync("ReceiveNewOrder", new
            {
                order.Id,
                order.TableId,
                order.Status,
                order.TotalAmount,
                order.OrderTime,
                order.IsError
            });
            await _hub.Clients.All.SendAsync("TableOccupied", new
            {
                table.Id,
                table.TableNumber,
                table.Status

            });


            return dto;
        }

        // ================= UPDATE =================
        public async Task<bool> UpdateAsync(int id, OrderDTO dto)
        {
            var table = await _context.Tables.FindAsync(dto.TableId);
            if (table == null)
            {
                throw new Exception("TableId không tồn tại");
            }
            if (dto.Status == "Completed" || dto.Status == "Canceled")
            {
                table.Status = "Available";
            }


            var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            order.TableId = dto.TableId;
            order.Status = dto.Status;
            order.Note = dto.Note;
            order.UpdateTime = DateTime.Now;

            // Remove old items
            _context.OrderItems.RemoveRange(order.OrderItems);

            // Add new items
            order.OrderItems = dto.Items?.Select(i => new OrderItems
            {
                MenuItemId = i.MenuItemId,
                Quantity = i.Quantity,
                Price = i.Price,
                Note = i.Note
            }).ToList() ?? new List<OrderItems>();

            order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);



            await _context.SaveChangesAsync();
            //  CHECK OUT OF STOCK
            var menuItemIds = order.OrderItems
                .Select(i => i.MenuItemId)
                .ToList();

            order.IsError = await _context.MenuItems
                .AnyAsync(m => menuItemIds.Contains(m.Id) && !m.IsAvailable);
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("OrderUpdated", new
            {
                order.Id,
                order.Status,
                order.TotalAmount
            });
            await _hub.Clients.All.SendAsync("TableOccupied", new
            {
                table.Id,
                table.TableNumber,
                table.Status

            });
            return true;
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<OrderEditDTO?> GetEditAsync(int id)
        {
            var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderEditDTO
            {
                Id = order.Id,
                TableId = order.TableId,
                OrderTime = order.OrderTime,
                Status = order.Status,
                Note = order.Note,

                Items = order.OrderItems.Select(i => new OrderItemEditDTO
                {
                    Id = i.Id,
                    MenuItemId = i.MenuItemId,
                    IsAvailable = i.MenuItem.IsAvailable,
                    MenuItemName = i.MenuItem.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.MenuItem.ImageUrl
                }).ToList()
            };
        }

        public async Task UpdateAsync(OrderEditDTO dto)
        {

            var table = await _context.Tables.FindAsync(dto.TableId);
            if (table == null)
            {
                throw new Exception("TableId không tồn tại");
            }
            if (dto.Status == "Completed" || dto.Status == "Canceled")
            {
                table.Status = "Available";
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstAsync(o => o.Id == dto.Id);

            order.TableId = dto.TableId;
            order.Status = dto.Status;
            order.Note = dto.Note;
            order.UpdateTime = DateTime.Now;

            if (dto.Items != null)
            {
                foreach (var item in dto.Items)
                {
                    var existing = order.OrderItems
                        .FirstOrDefault(x => x.Id == item.Id);

                    // DELETE
                    if (item.IsDeleted)
                    {
                        if (existing != null)
                            _context.OrderItems.Remove(existing);

                        continue;
                    }

                    // UPDATE
                    if (existing != null)
                    {
                        existing.Quantity = item.Quantity;
                    }
                    else
                    {
                        order.OrderItems.Add(new OrderItems
                        {
                            MenuItemId = item.MenuItemId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Recalculate total
            order.TotalAmount = await _context.OrderItems
                .Where(i => i.OrderId == order.Id)
                .SumAsync(i => i.Quantity * i.Price);

            await _context.SaveChangesAsync();
            // Recalculate total
            order.TotalAmount = await _context.OrderItems
                .Where(i => i.OrderId == order.Id)
                .SumAsync(i => i.Quantity * i.Price);

            // ✅ CHECK OUT OF STOCK
            var menuItemIds = await _context.OrderItems
                .Where(i => i.OrderId == order.Id)
                .Select(i => i.MenuItemId)
                .ToListAsync();

            order.IsError = await _context.MenuItems
                .AnyAsync(m => menuItemIds.Contains(m.Id) && !m.IsAvailable);

            await _hub.Clients.All.SendAsync("TableOccupied", new
            {
                table.Id,
                table.TableNumber,
                table.Status

            });

            await _hub.Clients.All.SendAsync("OrderUpdated", new
            {
                id = order.Id,
                status = order.Status
            });

            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateOrderUpdateTimeAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            order.UpdateTime = DateTime.Now;
            _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("MenuItemNotAvailable", new
            {
                order.Id,
                order.TableId,
                order.Status,
                order.TotalAmount,
                order.OrderTime
            });
            return true;
        }
    }
}