using FoodOrdering.DTOs;
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoriesService _service;

        public CategoriesController(ICategoriesService service)
        {
            _service = service;
        }

        // =====================
        // GET: Index
        // =====================
        public async Task<IActionResult> Index(CategoriesQuery query)
        {
            var data = await _service.GetAllAsync(query);
            ViewBag.Query = query;
            return View(data);
        }

        // =====================
        // CREATE
        // =====================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest();

            await _service.CreateAsync(dto);
            return Ok();
        }

        // =====================
        // EDIT
        // =====================
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name)
        {
            var result = await _service.UpdateAsync(id, new CategoryDTO
            {
                Name = name
            });

            if (!result) return NotFound();

            return Ok();
        }

        // =====================
        // DELETE
        // =====================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return BadRequest("Category still has products");

            return Ok();
        }
    }
}