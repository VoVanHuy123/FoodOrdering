using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodOrdering.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly IMenuItemsService _menuService;
        private readonly ICategoriesService _categoriesService;
        private readonly IOrdersService _ordersService;
        private readonly ICloudinaryService _cloudinary;

        public MenuItemsController(
            IMenuItemsService menuService,
            ICloudinaryService cloudinary,
            ICategoriesService categoriesService,
            IOrdersService ordersService)
        {
            _menuService = menuService;
            _cloudinary = cloudinary;
            _categoriesService = categoriesService;
            _ordersService = ordersService;
        }

        // =====================
        // GET: MenuItems
        // =====================
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoriesService.GetAllAsync();

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }

        // =====================
        // POST: Create
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MenuItemDTO dto)
        {
            var categories = await _categoriesService.GetAllAsync();
            ViewBag.CategoryId =
                new SelectList(categories, "Id", "Name", dto.CategoryId);

            if (!ModelState.IsValid)
            {
                
                ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
                return View(dto);


            }

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);
            var categories = await _categoriesService.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", menu.CategoryId);
            if (menu == null)
                return NotFound();

            return View(menu);
        }

        // =====================
        // POST: Edit
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, MenuItemDTO dto)
        {
            var categories = await _categoriesService.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", dto.CategoryId);
            if (!ModelState.IsValid)
                return View(dto);

            if (dto.ImageFile != null)
            {
                dto.ImageUrl =
                    await _cloudinary.UploadImageAsync(dto.ImageFile);
            }
            

            var result = await _menuService.UpdateAsync(id, dto);
            if (!result["IsUpdate"])
                return NotFound();
            if (result["isUpdateAvailable"])
            {
                await _ordersService.UpdateOrdersUpdateTimeByMenuItemNotAvailableAsync(id, result["isUpdateOrderError"]);
            }
               


            return RedirectToAction(nameof(Index));
        }

        // =====================
        // GET: Delete
        // =====================
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _menuService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}