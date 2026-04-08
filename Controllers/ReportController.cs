// Controllers/ReportController.cs
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
    using Microsoft.AspNetCore.Mvc;
        public class ReportsController : Controller
        {
            public IActionResult Index()
            {
                return View();
            }
        }
    
}