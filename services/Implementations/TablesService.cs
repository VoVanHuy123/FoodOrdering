using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.services.Implementations
{
    public class TablesService : ITablesService
    {
        private readonly FoodOrderingContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IConfiguration _config;
        public TablesService(FoodOrderingContext context, ICloudinaryService cloudinaryService, IConfiguration config)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _config = config;
        }

        public async Task<TableDTO> CreateTable(TableDTO dto)
        {
            var table = new Tables
            {
                TableNumber = dto.TableNumber,
                Status = "Available"
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            // URL khách sẽ mở
            var menuUrl = $"{_config["AppUrl"]}?tablesId={table.Id}";

            // tạo QR
            var qrBytes = _cloudinaryService.GenerateQRCode(menuUrl);

            // upload cloudinary
            var qrUrl = await _cloudinaryService.UploadQR(qrBytes);

            table.QRCode = qrUrl;

            await _context.SaveChangesAsync();

            return new TableDTO
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Status = table.Status,
                QRCode = table.QRCode
            };
        }

        public async Task<TableDTO> GetByTableNameAsync(string tableName)
        {
            var results = await _context.Tables.Where(t => t.TableNumber.ToString() == tableName)
                .Select(t => new TableDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    QRCode = t.QRCode,
                    Status = t.Status
                }).FirstOrDefaultAsync();
            return results;
            //var tableExists = await _context.Tables.AnyAsync(t => t.Id == dto.TableId);
            //if (!tableExists)
            //{
            //    throw new Exception("TableId không tồn tại");
            //}

        }
    }
}
