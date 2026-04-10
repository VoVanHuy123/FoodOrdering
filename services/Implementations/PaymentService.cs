using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FoodOrdering.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly FoodOrderingContext _context;
        private readonly IOrdersService _ordersService;

        public PaymentService(FoodOrderingContext context, IOrdersService ordersService)
        {
            _context = context;
            _ordersService = ordersService;
        }

        public async Task<bool> CreatePaymentAsync(PaymentDTO dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null) return false;

            // tránh tạo trùng
            var existed = await _context.Payments
                .AnyAsync(p => p.OrderId == dto.OrderId);

            if (existed) return false;

            var payment = new Payments
            {
                OrderId = dto.OrderId,
                Amount = order.TotalAmount,
                PaymentMethod = dto.PaymentMethod ?? "VNPAY",
                PaymentTime = DateTime.Now,
                Status = dto.Status ?? "Success"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}