using FoodOrdering.DTOs;

namespace FoodOrdering.services.Interfaces
{
    public interface ITablesService
    {
        Task <List<TableDTO>> GetAllTablesAsync(TablesQuery query);
        Task<bool> DeleteTableAsync(int id);
        Task <TableDTO> GetTableByIdAsync(int id);
        Task RecreateQACode(int id);
        Task<bool> UpdateTableAsync(int id, TableDTO dto);
        Task<TableDTO> GetByTableNameAsync(string tableName);
        Task<TableDTO> CreateTable(TableDTO table);
        Task<TableEntryDTO> ValidateTableEntryAsync(int tableId);
    }
}
