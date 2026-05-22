// FoodOrdering/ControllerAPIs/PaymentsControllerAPI.cs
using FoodOrdering.DTOs;
using FoodOrdering.services.Interfaces;
using FoodOrdering.Services.Implementations;
using FoodOrdering.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FoodOrdering.ControllerAPIs
{
    [ApiController]
    [Route("api/payments")]
    [AllowAnonymous]
    public class PaymentsControllerAPI : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly IPaymentService _paymentService;


        public PaymentsControllerAPI(IOrdersService ordersService, IPaymentService paymentService)
        {
            _ordersService = ordersService;
            _paymentService = paymentService;
        }

        [HttpPost("vnpay/create-url")]
        public async Task<IActionResult> CreateVnPayUrl([FromBody] VnPayRequest req)
        {
            var order = await _ordersService.GetByIdAsync(req.OrderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng");

            // Require params for VNPAY
            string vnp_TmnCode = Environment.GetEnvironmentVariable("VNPAY_TMN_CODE") ?? throw new InvalidOperationException("Missing VnpTmnCode configuration.");
            string vnp_HashSecret = Environment.GetEnvironmentVariable("VNPAY_HASH_SECRET") ?? throw new InvalidOperationException("Missing VnpHashSecret configuration.");
            string vnp_Url = Environment.GetEnvironmentVariable("VNPAY_BASE_URL") ?? throw new InvalidOperationException("Missing VnpBaseUrl configuration.");
            string returnBase = Environment.GetEnvironmentVariable("FrontendUrl") ?? throw new InvalidOperationException("Missing VNPAY_RETURN_URL_BASE");
            var vnp_Returnurl = $"{returnBase}/?tableId={order.TableId}";

            var vnp_Params = new SortedList<string, string>(new VnPayCompare())
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((long)order.TotalAmount * 100).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", "Thanh toan don hang " + order.Id },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", vnp_Returnurl },
                { "vnp_TxnRef", order.Id.ToString() + "_" + DateTime.Now.Ticks.ToString() }
            };

            // Build query string
            StringBuilder data = new();
            foreach (KeyValuePair<string, string> kv in vnp_Params)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString().TrimEnd('&');

            // Generate secure hash
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(vnp_HashSecret));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            string vnp_SecureHash = Convert.ToHexStringLower(hashValue);

            // Final payment URL
            string paymentUrl = vnp_Url + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;

            return Ok(new { url = paymentUrl });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO dto)
        {
            var result = await _paymentService.CreatePaymentAsync(dto);

            if (!result)
                return BadRequest("Create payment failed");

            return Ok("Payment created");
        }


    }

        public class VnPayRequest { public int OrderId { get; set; } }

        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return string.Compare(x, y, StringComparison.Ordinal);
            }
        }
  


    

    }   