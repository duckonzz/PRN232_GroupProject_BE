using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.StatisticsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
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

        /// <summary>
        /// Get monthly statistics of consultations.
        /// Filterable by year (required), and optionally by month, consultant, and status
        /// </summary>
        [HttpGet("consultations/statistics")]
        public async Task<IActionResult> GetConsultationStatistics([FromQuery] StatisticFilterRequest request)
        {
            var result = await _service.GetConsultationStatisticsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Get monthly statistics of health test bookings.
        /// Filterable by year (required), and optionally by month, health test, and status
        /// </summary>
        [HttpGet("test-bookings/statistics")]
        public async Task<IActionResult> GetTestBookingStatistics([FromQuery] StatisticFilterRequest request)
        {
            var result = await _service.GetTestBookingStatisticsAsync(request);
            return Ok(result);
        }
    }
}
