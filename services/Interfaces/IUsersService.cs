using FoodOrdering.DTOs;
namespace FoodOrdering.services.Interfaces
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetByIdAsync(int id);

        Task<UserDTO> CreateAsync(UserCreateDTO dto);

        Task<bool> UpdateAsync(int id, UserUpdateDTO dto);

        Task<bool> DeleteAsync(int id);

        Task<UserDTO?> LoginAsync(string username, string password);
    }
}
