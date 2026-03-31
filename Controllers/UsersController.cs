using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();

            if (users == null)
                return Content("Users NULL");

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

        // =====================
        // LOGIN
        // =====================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userService.LoginAsync(
                dto.Username,
                dto.Password);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            // lưu session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("role", user.Role);

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}