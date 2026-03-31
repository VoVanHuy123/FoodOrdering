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
    }
}
