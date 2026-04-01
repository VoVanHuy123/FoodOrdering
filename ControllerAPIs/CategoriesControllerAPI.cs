using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesControllerAPI : Controller
    {
        private readonly ICategoriesService _cateService;

        public CategoriesControllerAPI(ICategoriesService cateService)
        {
            _cateService = cateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _cateService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _cateService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

    }
}
