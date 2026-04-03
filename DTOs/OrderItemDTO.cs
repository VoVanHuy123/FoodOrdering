namespace FoodOrdering.DTOs
{
    public class OrderItemDTO
    {
        public int MenuItemId { get; set; }

        public string? MenuItemName { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Note { get; set; }
    }
}
