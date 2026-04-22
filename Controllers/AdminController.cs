using FoodOrdering.Models;
using FoodOrdering.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly NetworkAccessService _networkAccessService;

        public AdminController(NetworkAccessService networkAccessService)
        {
            _networkAccessService = networkAccessService;
        }

        [HttpGet]
        public IActionResult EditNetworkSettings()
        {
            var settings = _networkAccessService.GetSettings();
            return View(settings);
        }

        [HttpPost]
        public IActionResult EditNetworkSettings(NetworkAccessSettings model)
        {
            if (ModelState.IsValid)
            {
                _networkAccessService.UpdateSettings(model);
                TempData["Message"] = "Network settings have been updated.";
                return RedirectToAction("EditNetworkSettings");
            }
            return View(model);
        }
    }
}
