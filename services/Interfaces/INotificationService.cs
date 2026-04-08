namespace FoodOrdering.services.Interfaces
{
    public interface INotificationService
    {
        Task CreateCallStaffNotify(int tableId);
    }
}
