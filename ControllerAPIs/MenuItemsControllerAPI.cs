using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsControllerAPI : Controller
    {
        private readonly IMenuItemsService _menuService;
        private readonly ICloudinaryService _cloudinary;

        public MenuItemsControllerAPI(
            IMenuItemsService menuService,
            ICloudinaryService cloudinary)
        {
            _menuService = menuService;
            _cloudinary = cloudinary;
        }
        // =====================
        // GET ALL MENU
        // =====================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MenuItemQueryDTO query)
        {
            var result = await _menuService.GetAllAsync(query);
            return Ok(result);
        }
        // =====================
        // GET BY ID
        // =====================
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
                return NotFound();

            return Ok(menu);
        }
    }
}
