using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        // =====================
        // GET: Users
        // =====================
        public async Task<IActionResult> Index(UsersQuery query)
        {
            var users = await _userService.GetAllAsync(query);

            if (users == null)
                return Content("Users NULL");

            ViewBag.Query = query;
            return View(users);
        }

        // =====================
        // GET: Details
        // =====================
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
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
        public async Task<IActionResult> Create(UserCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _userService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        // =====================
        // GET: Edit
        // =====================
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // =====================
        // POST: Edit
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _userService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =====================
        // GET: Delete
        // =====================
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // =====================
        // POST: Delete
        // =====================
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

    
    }
}