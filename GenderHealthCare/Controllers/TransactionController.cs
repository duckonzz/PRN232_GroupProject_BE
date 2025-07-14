using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUserContextService _userContextService;

        public TransactionController(IPaymentService paymentService, IUserContextService userContextService)
        {
            _paymentService = paymentService;
            _userContextService = userContextService;
        }

        [HttpGet("my-payments")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = _userContextService.GetUserId();
            var payments = await _paymentService.GetUserPaymentsAsync(userId);
            return Ok(BaseResponseModel<List<PaymentResponseModel>>.OkDataResponse(payments, "Retrieved user transactions list successfully"));
        }
    }
}
