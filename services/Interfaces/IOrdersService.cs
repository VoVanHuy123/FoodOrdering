using FoodOrdering.DTOs;
using FoodOrdering.Models;

namespace FoodOrdering.services.Interfaces
{
    public interface IOrdersService
    {
        Task<PagedResult<OrderDTO>> GetAllAsync(OrderQuery query);
        Task<OrderDTO?> GetByIdAsync(int id);
        Task<OrderDTO> CreateAsync(OrderDTO dto);
        Task<bool> UpdateAsync(int id, OrderDTO dto);
        Task<bool> DeleteAsync(int id);

        Task<OrderEditDTO?> GetEditAsync(int id);
        Task UpdateAsync(OrderEditDTO dto);
        Task UpdateOrdersUpdateTimeByMenuItemNotAvailableAsync(int menuItemId, bool isErrorUpdate);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
    }
}