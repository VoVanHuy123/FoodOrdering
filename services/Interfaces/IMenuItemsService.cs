using FoodOrdering.DTOs;

namespace FoodOrdering.services.Interfaces
{
    public interface IMenuItemsService
    {
        Task<PagedResult<MenuItemDTO>> GetAllAsync(MenuItemQueryDTO query);
        Task<List<MenuItemDTO>> GetAllAsync();
        Task<MenuItemDTO?> GetByIdAsync(int id);
        Task<MenuItemDTO> CreateAsync(MenuItemDTO dto);
        Task<bool> UpdateAsync(int id, MenuItemDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
