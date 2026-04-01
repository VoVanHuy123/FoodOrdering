using FoodOrdering.DTOs;

namespace FoodOrdering.services.Interfaces
{
    public interface ITablesService
    {
        Task<TableDTO> GetByTableNameAsync(string tableName)    ;
    }
}
