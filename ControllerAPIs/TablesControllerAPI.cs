using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs.API
{
    [Route("api/tables")]
    [ApiController]
    public class TablesControllerAPI : Controller
    {
        private readonly ITablesService _tableService;

        public TablesControllerAPI(
            ITablesService tableService,
            ICloudinaryService cloudinary)
        {
            _tableService = tableService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] String tableName)
        {
            var result = await _tableService.GetByTableNameAsync(tableName);
            return Ok(result);
        }
    }
}
