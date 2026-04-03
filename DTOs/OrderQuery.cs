namespace FoodOrdering.DTOs
{
    public class OrderQuery
    {
        public int? TableNumber { get; set; }
        public string? Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }
}
