namespace FoodOrdering.Models
{
    public class Payments
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Orders? Order { get; set; }

        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string? Status { get; set; }
    }
}
