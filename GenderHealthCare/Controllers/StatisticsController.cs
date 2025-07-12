using GenderHealthCare.Contract.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;
        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpGet("customer-count")]
        public async Task<IActionResult> GetCustomerCount()
        {
            var count = await _service.CountAllCustomersAsync();
            return Ok(count);
        }

        /// <summary>
        /// Thống kê số lượng lịch xét nghiệm đã được đặt
        /// (IsBooked == true và BookedByUserId != null trong TestSlot).
        /// </summary>
        [HttpGet("booked-testslot-count")]
        public async Task<IActionResult> GetBookedTestSlotCount()
        {
            var result = await _service.CountBookedTestSlotsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Thống kê số lượng lịch tư vấn đã được đặt
        /// (IsBooked == true và BookedByUserId != null trong AvailableSlot).
        /// </summary>
        [HttpGet("booked-available-slot-count")]
        public async Task<IActionResult> GetBookedAvailableSlotCount()
        {
            var result = await _service.CountBookedAvailableSlotsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
