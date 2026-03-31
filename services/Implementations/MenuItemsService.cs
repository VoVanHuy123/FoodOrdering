using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class MenuItemsService : IMenuItemsService
{
    private readonly FoodOrderingContext _context;

    public MenuItemsService(FoodOrderingContext context)
    {
        _context = context;
    }

    // ================= GET ALL =================
    public async Task<List<MenuItemDTO>> GetAllAsync()
    {
        return await _context.MenuItems
            .Select(x => new MenuItemDTO
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                CategoryId = x.CategoryId
            })
            .ToListAsync();
    }

    // ================= GET BY ID =================
    public async Task<MenuItemDTO?> GetByIdAsync(int id)
    {
        var item = await _context.MenuItems.FindAsync(id);

        if (item == null) return null;

        return new MenuItemDTO
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            CategoryId = item.CategoryId
        };
    }

    // ================= CREATE =================
    public async Task<MenuItemDTO> CreateAsync(MenuItemDTO dto)
    {
        var menuItem = new MenuItems
        {
            Name = dto.Name,
            Price = dto.Price,
            CategoryId = dto.CategoryId
        };

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        dto.Id = menuItem.Id;
        return dto;
    }

    // ================= UPDATE =================
    public async Task<bool> UpdateAsync(int id, MenuItemDTO dto)
    {
        var item = await _context.MenuItems.FindAsync(id);

        if (item == null) return false;

        item.Name = dto.Name;
        item.Price = dto.Price;
        item.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();
        return true;
    }

    // ================= DELETE =================
    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.MenuItems.FindAsync(id);

        if (item == null) return false;

        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }
}