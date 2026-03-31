namespace FoodOrdering.DTOs
{
    public class MenuItemDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public bool IsAvailable { get; set; }
    }
}
