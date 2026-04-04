using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class MenuItemsService : IMenuItemsService
{
    private readonly FoodOrderingContext _context;
    private readonly ICategoriesService _categoriesService;

    public MenuItemsService(FoodOrderingContext context, ICategoriesService categoriesService)
    {
        _context = context;
        _categoriesService = categoriesService;
    }
    

    // ================= GET ALL =================
    public async Task<List<MenuItemDTO>> GetAllAsync() 
    { 
        return await _context.MenuItems.Select(x => new MenuItemDTO 
        { 
            Id = x.Id,
            Name = x.Name,
            Price = x.Price, 
            CategoryId = x.CategoryId 
        }).ToListAsync(); 
    }
    public async Task<PagedResult<MenuItemDTO>> GetAllAsync(MenuItemQueryDTO query)
    {
        var menuQuery = _context.MenuItems
            .Include(x => x.Category)
            .AsQueryable();

        // ===== SEARCH NAME =====
        
        if (!string.IsNullOrEmpty(query.Search))
        {
            menuQuery = menuQuery.Where(x =>
                EF.Functions.Collate(x.Name!, "SQL_Latin1_General_CP1_CI_AI")
                .Contains(query.Search));
        }

        // ===== FILTER CATEGORY =====
        if (query.CategoryId.HasValue)
        {
            menuQuery = menuQuery
                .Where(x => x.CategoryId == query.CategoryId.Value);
        }

        // ===== TOTAL COUNT =====
        var totalItems = await menuQuery.CountAsync();

        // ===== PAGINATION =====
        var items = await menuQuery
            .OrderByDescending(x => x.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new MenuItemDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
                CategoryId = x.CategoryId,
                CategoryName = x.Category!.Name,
                IsAvailable = x.IsAvailable
            })
            .ToListAsync();

        return new PagedResult<MenuItemDTO>
        {
            TotalItems = totalItems,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Items = items
        };
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
            IsAvailable = item.IsAvailable,
            Description = item.Description,
            ImageUrl = item.ImageUrl,
            CategoryName = (await _context.Categories.FindAsync(item.CategoryId))?.Name,
            CategoryId = item.CategoryId
        };
    }

    // ================= CREATE =================
    public async Task<MenuItemDTO> CreateAsync(MenuItemDTO dto)
    {
        var menuItem = new MenuItems(dto);
        //{
        //    Name = dto.Name,
        //    Price = dto.Price,
        //    CategoryId = dto.CategoryId,
        //    IsAvailable = dto.IsAvailable,
        //    Description = dto.Description,
        //    ImageUrl = dto.ImageUrl,
        //};

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        dto.Id = menuItem.Id;
        return dto;
    }

    // ================= UPDATE =================
    public async Task<Dictionary<string,bool>> UpdateAsync(int id, MenuItemDTO dto)
    {
        var item = await _context.MenuItems.FindAsync(id);

        if (item == null) return new Dictionary<string, bool>
        {
            { "IsUpdate", false },
            { "IsUpdateAvailable", false }
        };
        bool isUpdateOrderError = false;
        bool isUpdateAvailable = false;
        if (item.IsAvailable == true && item.IsAvailable != dto.IsAvailable)
        {
            isUpdateOrderError = true;
            isUpdateAvailable = true;
        }
        if(item.IsAvailable == false && item.IsAvailable != dto.IsAvailable)
        {
            isUpdateOrderError = false;
            isUpdateAvailable = true;
        }
        
        item.Update(dto);

        await _context.SaveChangesAsync();
        return new Dictionary<string, bool>
        {
            {  "IsUpdate", true  },
            { "isUpdateOrderError", isUpdateOrderError },
            {"isUpdateAvailable", isUpdateAvailable }
        };
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