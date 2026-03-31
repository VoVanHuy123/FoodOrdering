using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.services.Implementations
{
    public class OrdersService : IOrdersService
    {
        private readonly FoodOrderingContext _context;

        public OrdersService(FoodOrderingContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        public async Task<List<OrderDTO>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    TableId = o.TableId,
                    OrderTime = o.OrderTime,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    Note = o.Note,
                    Items = o.OrderItems.Select(i => new OrderItemDTO
                    {
                        MenuItemId = i.MenuItemId,
                        MenuItemName = i.MenuItem != null ? i.MenuItem.Name : null,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        Note = i.Note
                    }).ToList()
                })
                .ToListAsync();
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
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Note = i.Note
                }).ToList()
            };
        }

        // ================= CREATE =================
        public async Task<OrderDTO> CreateAsync(OrderDTO dto)
        {
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

            // Tính TotalAmount
            order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            dto.Id = order.Id;
            dto.OrderTime = order.OrderTime;
            dto.TotalAmount = order.TotalAmount;

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
    }
}