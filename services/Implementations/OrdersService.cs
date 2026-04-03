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
                .OrderByDescending(o => o.OrderTime)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    TableId = o.TableId,
                    OrderTime = o.OrderTime,
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
                    MenuItemName = i.MenuItem != null ? i.MenuItem.Name : null,
                    ImageUrl = i.MenuItem.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Note = i.Note
                }).ToList()
            };
        }

        // ================= CREATE =================
        public async Task<OrderDTO> CreateAsync(OrderDTO dto)
        {
            var tableExists = await _context.Tables.AnyAsync(t => t.Id == dto.TableId);
            if (!tableExists)
            {
                throw new Exception("TableId không tồn tại");
            }
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

            dto.Id = order.Id;
            dto.OrderTime = order.OrderTime;
            dto.TotalAmount = order.TotalAmount;

            
            // SIGNALR REALTIME
            await _hub.Clients.All.SendAsync("ReceiveNewOrder", new
            {
                order.Id,
                order.TableId,
                order.Status,
                order.TotalAmount,
                order.OrderTime
            });

            return dto;
        }

        // ================= UPDATE =================
        public async Task<bool> UpdateAsync(int id, OrderDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            order.TableId = dto.TableId;
            order.Status = dto.Status;
            order.Note = dto.Note;

            // Xóa items cũ
            _context.OrderItems.RemoveRange(order.OrderItems);

            // Thêm items mới
            order.OrderItems = dto.Items?.Select(i => new OrderItems
            {
                MenuItemId = i.MenuItemId,
                Quantity = i.Quantity,
                Price = i.Price,
                Note = i.Note
            }).ToList() ?? new List<OrderItems>();

            order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);

            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("OrderUpdated", new
            {
                order.Id,
                order.Status,
                order.TotalAmount
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
                    MenuItemName = i.MenuItem.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.MenuItem.ImageUrl
                }).ToList()
            };
        }

        public async Task UpdateAsync(OrderEditDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstAsync(o => o.Id == dto.Id);

            order.TableId = dto.TableId;
            order.Status = dto.Status;
            order.Note = dto.Note;

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
        }
    }
}