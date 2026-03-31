namespace FoodOrdering.DTOs
{
    public class PaymentDTO
    {
        public int OrderId { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
    }
}
