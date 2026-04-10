using FoodOrdering.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO?> LoginAsync(string username, string password);
    }
}
