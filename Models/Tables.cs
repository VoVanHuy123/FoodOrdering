namespace FoodOrdering.Models
{
    public class Tables
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string? QRCode { get; set; }
        public string? Status { get; set; }

        public ICollection<Orders>? Orders { get; set; }
    }
}
