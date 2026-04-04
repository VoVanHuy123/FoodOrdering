namespace FoodOrdering.DTOs
{
    public class TableDTO
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string? QRCode { get; set; }
        public string? Status { get; set; }
    }
    public class TablesQuery
    {
        public string? Status { get; set; }
        public int? TableNumber { get; set; }
    }
}
