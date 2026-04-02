namespace FoodOrdering.services.Interfaces
{
    public interface ICloudinaryService
    {
        byte[] GenerateQRCode(string url);
        Task<string> UploadQR(byte[] imageBytes);
         Task<string> UploadImageAsync(IFormFile file);
    }
}
