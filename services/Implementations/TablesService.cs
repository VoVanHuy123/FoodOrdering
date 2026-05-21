using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        public async Task RecreateQACode(int id)
        {
            var table = await _context.Tables.FindAsync(id);

            var menuUrl = $"{_config["AppUrl"]}?tableId={table.Id}";

            // tạo QR
            var qrBytes = _cloudinaryService.GenerateQRCode(menuUrl);

            // upload cloudinary
            var qrUrl = await _cloudinaryService.UploadQR(qrBytes);

            table.QRCode = qrUrl;

            await _context.SaveChangesAsync();

        }

        public async Task<bool> DeleteTableAsync(int id)
        {
            var table = await _context.Tables
                 .Include(t => t.Orders)
                 .FirstOrDefaultAsync(t => t.Id == id);

            if (table == null)
                return false;

            // tránh xóa bàn đang có order
            if (table.Orders != null && table.Orders.Any())
                throw new Exception("Table already has orders.");

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<TableDTO> GetTableByIdAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                return null;
            return new TableDTO
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                QRCode = table.QRCode,
                Status = table.Status
            };
        }

        public async Task<List<TableDTO>> GetAllTablesAsync(TablesQuery query)
        {
            var tables = _context.Tables.AsQueryable();

            // filter status
            if (!string.IsNullOrEmpty(query?.Status))
            {
                tables = tables.Where(t => t.Status == query.Status);
            }

            // filter table number
            if (query?.TableNumber != null)
            {
                tables = tables.Where(t => t.TableNumber == query.TableNumber);
            }

            return await tables
                .OrderBy(t => t.TableNumber)
                .Select(t => new TableDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    QRCode = t.QRCode,
                    Status = t.Status
                })
                .ToListAsync();
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

        public async Task<bool> UpdateTableAsync(int id, TableDTO dto)
        {
            var table = await _context.Tables.FindAsync(id);

            if (table == null)
                return false;

            table.TableNumber = dto.TableNumber;
            table.Status = dto.Status;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
