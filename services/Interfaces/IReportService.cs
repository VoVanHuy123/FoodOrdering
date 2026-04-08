using FoodOrdering.DTOs;

namespace FoodOrdering.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportDTO>> GetRevenueAsync(DateTime from, DateTime to);
        Task<List<ReportDTO>> GetOrderCountAsync(DateTime from, DateTime to);
        Task<List<ReportDTO>> GetTopFoodsAsync(DateTime from, DateTime to);
    }
}