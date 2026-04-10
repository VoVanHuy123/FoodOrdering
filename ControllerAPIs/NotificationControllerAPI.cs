using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs
{
    [ApiController]
    [Route("api/notifications")]
    [AllowAnonymous]
    public class NotificationControllerAPI : Controller
    {
        private readonly INotificationService _notifyService;

        public NotificationControllerAPI(INotificationService notifyService)
        {
            this._notifyService = notifyService;
        }
        [HttpPost("call-staff/{tableId}")]
        public async Task<IActionResult> CreateCallStaffNotify(int tableId)
        {
            await _notifyService.CreateCallStaffNotify(tableId);
            return Ok();
        }
    }
}
