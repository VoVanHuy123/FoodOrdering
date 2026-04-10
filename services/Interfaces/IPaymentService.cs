using FoodOrdering.DTOs;

namespace FoodOrdering.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> CreatePaymentAsync(PaymentDTO dto);
    }
}