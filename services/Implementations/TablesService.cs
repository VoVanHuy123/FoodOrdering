using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.services.Implementations
{
    public class TablesService : ITablesService
    {
        private readonly FoodOrderingContext _context;
        public TablesService(FoodOrderingContext context)
        {
            _context = context;
        }
        public async Task<TableDTO> GetByTableNameAsync(string tableName)
        {
            return await _context.Tables.Where(t => t.TableNumber.ToString() == tableName)
                .Select(t => new TableDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    QRCode = t.QRCode,
                    Status = t.Status
                }).FirstOrDefaultAsync();
        }
    }
}
