namespace FoodOrdering.DTOs
{
    public class OrderItemEditDTO
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; }

        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsDeleted { get; set; }
    }
}
