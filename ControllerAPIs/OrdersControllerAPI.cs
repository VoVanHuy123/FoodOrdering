using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FoodOrdering.Hubs;
using Microsoft.AspNetCore.Authorization;

namespace FoodOrdering.ControllerAPIs
{
    [ApiController]
    [Route("api/orders")]
    [AllowAnonymous]
    public class OrdersControllerAPI : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrdersControllerAPI(IOrdersService ordersService, IHubContext<OrderHub> hubContext)
        {
            _ordersService = ordersService;
            _hubContext = hubContext;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> GetAll(OrderQuery query)
        {
            var orders = await _ordersService.GetAllAsync(query);
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
            try
            {

                if (dto == null)
                    return BadRequest("Order data is null.");

                var createdOrder = await _ordersService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById),
                    new { id = createdOrder.Id },
                    createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
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

        // ================= CALL STAFF FOR CASH =================
        [HttpPost("call-staff")]
        public async Task<IActionResult> CallStaffForCash([FromBody] CallStaffRequest request)
        {
            if (string.IsNullOrEmpty(request.TableId))
                return BadRequest("TableId is required");

            // Send a SignalR signal to all administrators currently viewing the website
            await _hubContext.Clients.All.SendAsync("ReceiveCashAlert", new
            {
                tableId = request.TableId,
                message = $"Yêu cầu thanh toán tiền mặt!"
            });

            return Ok(new { success = true, message = "Đã thông báo cho nhân viên" });
        }
        
        // ================= UPDATE STATUS =================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
        {
           var result = await _ordersService.UpdateOrderStatusAsync(id, req.Status);
    
            if (!result)
                return NotFound($"Không tìm thấy đơn hàng với Id = {id} hoặc cập nhật thất bại.");

            return Ok(new { success = true, message = "Cập nhật thành công" });
        }
    }

    public class CallStaffRequest
    {
        public required string TableId { get; set; }
    }

    public class UpdateStatusRequest
    {
        public required string Status { get; set; }
    }
}