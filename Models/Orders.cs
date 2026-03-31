namespace FoodOrdering.Models
{
    public class Orders
    {
        public int Id { get; set; }

        public int TableId { get; set; }
        public Tables? Table { get; set; }

        public DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }

        public ICollection<OrderItems>? OrderItems { get; set; }
        public ICollection<Payments>? Payments { get; set; }
    }
}
