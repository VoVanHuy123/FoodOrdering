using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodOrdering.services.Interfaces;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
namespace FoodOrdering.services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]
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
