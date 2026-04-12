using System.Diagnostics;
using FoodOrdering.Context;
using FoodOrdering.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
    public class HomeController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        private readonly FoodOrderingContext _context;

        public HomeController(FoodOrderingContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var foods = _context.MenuItems
                .OrderByDescending(m => m.Id)
                .ToList();
            return View(foods);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
