namespace FoodOrdering.Models
{
    public class OrderItems
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Orders? Order { get; set; }

        public int MenuItemId { get; set; }
        public MenuItems? MenuItem { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Note { get; set; }
    }
}
