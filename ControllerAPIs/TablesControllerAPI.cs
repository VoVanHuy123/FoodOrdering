using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.ControllerAPIs.API
{
    [ApiController]
    [Route("api/tables")]
    [AllowAnonymous]
    public class TablesControllerAPI : ControllerBase
    {
        private readonly ITablesService _tableService;

        public TablesControllerAPI(ITablesService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string tableName)
        {
            var result = await _tableService.GetByTableNameAsync(tableName);
            return Ok(result);
        }

        /// <summary>
        /// FE gọi khi mở app từ QR (?tableId=). Chỉ cho vào khi Status = Available.
        /// </summary>
        [HttpGet("{id:int}/entry")]
        public async Task<IActionResult> ValidateEntry(int id)
        {
            var result = await _tableService.ValidateTableEntryAsync(id);
            if (result.Table == null)
                return NotFound(result);

            return Ok(result);
        }
    }
}
