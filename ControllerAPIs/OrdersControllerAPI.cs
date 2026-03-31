using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersControllerAPI : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersControllerAPI(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> GetAll()
        {
            var orders = await _ordersService.GetAllAsync();
            return Ok(orders);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetById(int id)
        {
            var order = await _ordersService.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Order with Id = {id} not found.");

            return Ok(order);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> Create([FromBody] OrderDTO dto)
        {
            if (dto == null)
                return BadRequest("Order data is null.");

            var createdOrder = await _ordersService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDTO dto)
        {
            if (dto == null)
                return BadRequest("Order data is null.");

            var result = await _ordersService.UpdateAsync(id, dto);
            if (!result)
                return NotFound($"Order with Id = {id} not found.");

            return NoContent();
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ordersService.DeleteAsync(id);
            if (!result)
                return NotFound($"Order with Id = {id} not found.");

            return NoContent();
        }
    }
}