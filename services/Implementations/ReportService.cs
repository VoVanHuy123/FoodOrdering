using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Hubs;
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly FoodOrderingContext _context;
        private readonly IHubContext<OrderHub> _hub;
        public ReportService(FoodOrderingContext context,
            IHubContext<OrderHub> hub)
        {
            _context = context;
        }

        // Doanh thu theo ngày
        public async Task<List<ReportDTO>> GetRevenueAsync(DateTime from, DateTime to)
        {
            var data = await _context.Orders
                     .Where(o => o.OrderTime >= from && o.OrderTime <= to
                                 && !o.IsError
                                 && o.Status == "Completed")
                     .GroupBy(o => o.OrderTime.Date)
                     .Select(g => new
                     {
                         Date = g.Key,
                         Value = g.Sum(x => x.TotalAmount)
                     })
                     .OrderBy(x => x.Date)
                     .ToListAsync();

            return data.Select(x => new ReportDTO
            {
                Label = x.Date.ToString("dd/MM"),
                Value = x.Value
            }).ToList();
        }

        // Số lượng đơn
        public async Task<List<ReportDTO>> GetOrderCountAsync(DateTime from, DateTime to)
        {
            var data = await _context.Orders
                .Where(o => o.OrderTime >= from && o.OrderTime <= to
                            && !o.IsError
                            && o.Status == "Completed")
                .GroupBy(o => o.OrderTime.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Value = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new ReportDTO
            {
                Label = x.Date.ToString("dd/MM"),
                Value = x.Value
            }).ToList();
        }

        // Top  bán chạy
        public async Task<List<ReportDTO>> GetTopFoodsAsync(DateTime from, DateTime to)
        {
            var data = await _context.OrderItems
               .Where(oi => oi.Order.OrderTime >= from && oi.Order.OrderTime <= to
                            && !oi.Order.IsError
                            && oi.Order.Status == "Completed")
               .GroupBy(oi => oi.MenuItem.Name)
               .Select(g => new
               {
                   Name = g.Key,
                   Quantity = g.Sum(x => (decimal)x.Quantity)
               })
               .OrderByDescending(x => x.Quantity)
               .Take(10)
               .ToListAsync();

            return data.Select(x => new ReportDTO
            {
                Label = x.Name,
                Value = x.Quantity
            }).ToList();
        }
    }
}