using FoodOrdering.Services.Interfaces;
using FoodOrdering.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportApiController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("Revenue")]
        public async Task<ActionResult<List<ReportDTO>>> GetRevenue(DateTime from, DateTime to)
        {
            var data = await _reportService.GetRevenueAsync(from, to);
            return Ok(data);
        }

        [HttpGet("Orders")]
        public async Task<ActionResult<List<ReportDTO>>> GetOrders(DateTime from, DateTime to)
        {
            var data = await _reportService.GetOrderCountAsync(from, to);
            return Ok(data);
        }

        [HttpGet("TopFoods")]
        public async Task<ActionResult<List<ReportDTO>>> GetTopFoods(DateTime from, DateTime to)
        {
            var data = await _reportService.GetTopFoodsAsync(from, to);
            return Ok(data);
        }
    }
}