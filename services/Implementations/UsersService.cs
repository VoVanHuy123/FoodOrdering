using BCrypt.Net;
using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly FoodOrderingContext _context;

        public UsersService(FoodOrderingContext context)
        {
            _context = context;
        }

        // =====================
        // GET ALL
        // =====================
        public async Task<List<UserDTO>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();
        }

        // =====================
        // GET BY ID
        // =====================
        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role
            };
        }

        // =====================
        // CREATE USER
        // =====================
        public async Task<UserDTO> CreateAsync(UserCreateDTO dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new Users
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Password = hashedPassword,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role
            };
        }

        // =====================
        // UPDATE
        // =====================
        public async Task<bool> UpdateAsync(int id, UserUpdateDTO dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return false;

            user.FullName = dto.FullName;
            user.Role = dto.Role;

            await _context.SaveChangesAsync();
            return true;
        }

        // =====================
        // DELETE
        // =====================
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        // =====================
        // LOGIN
        // =====================
        public async Task<UserDTO?> LoginAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            bool isValid = false;
            // nếu password đã là bcrypt
            if (user.Password.StartsWith("$2"))
            {
                isValid = BCrypt.Net.BCrypt.Verify(password.Trim(), user.Password);
            }
            else
            {
                // fallback: so sánh plain text
                if (user.Password == password)
                {
                    // hash lại để chuẩn hóa
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _context.SaveChangesAsync();
                    isValid = true;
                }
            }

            if (!isValid)
                return null;

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}