using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.VNPayModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;
        public VnPayController(IVnPayService vnPayService, IPaymentService paymentService)
        {
            _vnPayService = vnPayService;
            _paymentService = paymentService;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] VnPayPaymentRequest request)
        {
            // Lấy IP address thực và xử lý IPv6
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ipAddress == "::1")
                ipAddress = "127.0.0.1";

            var paymentUrl = _vnPayService.CreatePaymentUrl(request, ipAddress);
            var response = new VnPayPaymentResponse
            {
                PaymentUrl = paymentUrl
            };
            return Ok(response);
        }
       


    }
}
