namespace FoodOrdering.DTOs
{
    public class OrderEditDTO
    {
        public int Id { get; set; }

        public int TableId { get; set; }

        public DateTime OrderTime { get; set; }
       

        public string Status { get; set; }

        public string? Note { get; set; }

        public List<OrderItemEditDTO> Items { get; set; }
            = new();
    }
}
