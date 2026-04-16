using FoodOrdering.DTOs;

namespace FoodOrdering.Services.Interfaces
{
    public interface ICategoriesService
    {
        Task<List<CategoryDTO>> GetAllAsync(CategoriesQuery? query = null);

        Task<CategoryDTO?> GetByIdAsync(int id);

        Task<CategoryDTO> CreateAsync(CategoryDTO dto);

        Task<bool> UpdateAsync(int id, CategoryDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}