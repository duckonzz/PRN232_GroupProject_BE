using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.VNPayModels;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/vnpay")]
    public class VnPayCallbackController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public VnPayCallbackController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("save-payment")]
        public async Task<IActionResult> SavePayment([FromBody] VnPayCallbackRequest request)
        {
            try
            {
                await _paymentService.SaveVnPayResultAsync(request);
                return Ok(new { success = true, message = "Lưu giao dịch thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
