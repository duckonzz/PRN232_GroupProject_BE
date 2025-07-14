using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.StatisticsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Get total revenue and transaction count in a date range
        /// </summary>
        /// <param name="fromDate">Start date (optional).</param>
        /// <param name="toDate">End date (optional).</param>
        /// <returns>Revenue stats for successful payments.</returns>
        [HttpGet("statistics/revenue")]
        public async Task<IActionResult> GetRevenueStatistics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _service.GetRevenueStatisticsAsync(fromDate, toDate);
            return Ok(BaseResponseModel<RevenueStatisticsResponse>.OkDataResponse(result, "Revenue statistics fetched successfully"));
        }
    }
}
