using System.Drawing;
using System.Drawing.Imaging;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Settings;
using Microsoft.Extensions.Options;
using QRCoder;
namespace FoodOrdering.services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;

            if (string.IsNullOrEmpty(settings.CloudName) ||
                string.IsNullOrEmpty(settings.ApiKey) ||
                string.IsNullOrEmpty(settings.ApiSecret))
            {
                throw new Exception("Cloudinary settings is missing!");
            }

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length == 0)
                return "";

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result.SecureUrl.ToString();
        }
        public byte[] GenerateQRCode(string url)
        {
            using var qrGenerator = new QRCodeGenerator();

            var data = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new QRCode(data);

            using Bitmap bitmap = qrCode.GetGraphic(20);

            using MemoryStream ms = new MemoryStream();

            bitmap.Save(ms, ImageFormat.Png);

            return ms.ToArray();
        }
        public async Task<string> UploadQR(byte[] imageBytes)
        {
            using var stream = new MemoryStream(imageBytes);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("table_qr.png", stream),
                Folder = "tables_qr"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result.SecureUrl.ToString();
        }

    }
}
