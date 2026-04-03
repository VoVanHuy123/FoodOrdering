using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodOrdering.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly IMenuItemsService _menuService;
        private readonly ICategoriesService _categoriesService;
        private readonly ICloudinaryService _cloudinary;

        public MenuItemsController(
            IMenuItemsService menuService,
            ICloudinaryService cloudinary,
            ICategoriesService categoriesService)
        {
            _menuService = menuService;
            _cloudinary = cloudinary;
            _categoriesService = categoriesService;
        }

        // =====================
        // GET: MenuItems
        // =====================
        public async Task<IActionResult> Index(MenuItemQueryDTO query)
        {
            var menus = await _menuService.GetAllAsync(query);
            var categories = await _categoriesService.GetAllAsync();
            ViewBag.Categories = categories;
            if (menus == null)
                return Content("menus NULL");

            return View(menus);
        }

        // =====================
        // GET: Details
        // =====================
        public async Task<IActionResult> Details(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
                return NotFound();

            return View(menu);
        }

        // =====================
        // GET: Create
        // =====================
        public IActionResult Create()
        {
            return View();
        }

        // =====================
        // POST: Create
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItemDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // Upload Cloudinary
            if (dto.ImageFile != null)
            {
                dto.ImageUrl =
                    await _cloudinary.UploadImageAsync(dto.ImageFile);
            }

            await _menuService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        // =====================
        // GET: Edit
        // =====================
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
                return NotFound();

            return View(menu);
        }

        // =====================
        // POST: Edit
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuItemDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (dto.ImageFile != null)
            {
                dto.ImageUrl =
                    await _cloudinary.UploadImageAsync(dto.ImageFile);
            }

            var result = await _menuService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =====================
        // GET: Delete
        // =====================
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
                return NotFound();

            return View(menu);
        }

        // =====================
        // POST: Delete
        // =====================
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _menuService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}