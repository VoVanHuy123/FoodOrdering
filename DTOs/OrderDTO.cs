namespace FoodOrdering.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }

        public List<OrderItemDTO>? Items { get; set; }
        public bool IsError { get; set; }
    }
}
