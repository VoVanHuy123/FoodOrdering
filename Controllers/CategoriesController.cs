using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FoodOrdering.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoriesService _service;

        public CategoriesController(ICategoriesService service)
        {
            _service = service;
        }

        // GET: 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _service.CreateAsync(dto);

            return RedirectToAction("Index", "MenuItems");
        }
    }
}
