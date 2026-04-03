using System.Security.Claims;
using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FoodOrdering.services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUsersService _usersService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUsersService usersService, IHttpContextAccessor httpContextAccessor)
        {
            _usersService = usersService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDTO?> LoginAsync(string username,string password)
        {
            var user = await _usersService.LoginAsync(username, password);
            if (user == null) return null;

            // tạo cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return user;
        }
    }
}