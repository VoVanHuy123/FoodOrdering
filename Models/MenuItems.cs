using FoodOrdering.DTOs;
using Humanizer;

namespace FoodOrdering.Models
{
    public class MenuItems
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }

        public int CategoryId { get; set; }
        public Categories? Category { get; set; }

        public ICollection<OrderItems>? OrderItems { get; set; }

        public MenuItems() { }
        public MenuItems(MenuItemDTO dto)
        {
            this.Name = dto.Name;
            this.Price = dto.Price;
            this.CategoryId = dto.CategoryId;
            this.IsAvailable = dto.IsAvailable;
            this.Description = dto.Description;
            this.ImageUrl = dto.ImageUrl;
        }

        public void Update(MenuItemDTO dto)
        {
            this.Name = dto.Name;
            this.Price = dto.Price;
            this.CategoryId = dto.CategoryId;
            this.IsAvailable = dto.IsAvailable;
            this.Description = dto.Description;
            if (dto.ImageUrl != null)
            {
                this.ImageUrl = dto.ImageUrl;
            }
        }
    }
}
