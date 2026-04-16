using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.services.Implementations
{
    public class CategoriesService : ICategoriesService
    {
        private readonly FoodOrderingContext _context;

        public CategoriesService(FoodOrderingContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        public async Task<List<CategoryDTO>> GetAllAsync(CategoriesQuery? query = null)
        {
            var categories = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query?.Name))
            {
                var search = query.Name.Trim().ToLower();
                categories = categories.Where(c => c.Name!.ToLower().Contains(search));
            }

            return await categories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = _context.MenuItems.Count(m => m.CategoryId == c.Id)
                })
                .ToListAsync();
        }

        // ================= GET BY ID =================
        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        // ================= CREATE =================
        public async Task<CategoryDTO> CreateAsync(CategoryDTO dto)
        {
            var category = new Categories
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            dto.Id = category.Id;
            return dto;
        }

        // ================= UPDATE =================
        public async Task<bool> UpdateAsync(int id, CategoryDTO dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return false;

            category.Name = dto.Name;

            await _context.SaveChangesAsync();
            return true;
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return false;

            var hasProducts = await _context.MenuItems.AnyAsync(m => m.CategoryId == id);

            if (hasProducts)
                return false; 

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
