using FoodOrdering.Services.Interfaces;
using FoodOrdering.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(string range = "month", DateTime? from = null, DateTime? to = null)
        {
            DateTime fromDate, toDate;
            var now = DateTime.Now;

            switch (range)
            {
                case "day":
                    fromDate = new DateTime(now.Year, now.Month, now.Day);
                    toDate = now;
                    break;

                case "quarter":
                    int q = (now.Month - 1) / 3;
                    fromDate = new DateTime(now.Year, q * 3 + 1, 1);
                    toDate = now;
                    break;

                case "year":
                    fromDate = new DateTime(now.Year, 1, 1);
                    toDate = now;
                    break;

                case "custom":
                    fromDate = from ?? now.AddDays(-7);
                    toDate = to ?? now;
                    break;

                default: // month
                    fromDate = new DateTime(now.Year, now.Month, 1);
                    toDate = now;
                    break;
            }

            var model = new ReportViewModel
            {
                From = fromDate,
                To = toDate,
                Range = range,
                Revenue = await _reportService.GetRevenueAsync(fromDate, toDate),
                Orders = await _reportService.GetOrderCountAsync(fromDate, toDate),
                TopFoods = await _reportService.GetTopFoodsAsync(fromDate, toDate)
            };

            return View(model);
        }
    }
}