using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.AvailableSlotModels;
using GenderHealthCare.ModelViews.HealthTestScheduleModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthTestScheduleController : ControllerBase
    {
        private readonly IHealthTestScheduleService _healthTestScheduleService;

        public HealthTestScheduleController(IHealthTestScheduleService healthTestScheduleService)
        {
            _healthTestScheduleService = healthTestScheduleService;
        }

        // POST: api/HealthTestSchedule
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] HealthTestScheduleRequestModel model)
        {
            var userId = User.FindFirstValue("id");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _healthTestScheduleService.CreateScheduleAsync(model);
            return Ok(result);
        }

        // GET: api/HealthTestSchedule/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _healthTestScheduleService.GetScheduleByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/HealthTestSchedule
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _healthTestScheduleService.GetAllSchedulesAsync();
            return Ok(result);
        }

        // GET: api/HealthTestSchedule/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSchedules()
        {
            var currentDate = DateTime.Now;
            var result = await _healthTestScheduleService.GetActiveSchedulesAsync(currentDate);
            return Ok(result);
        }

        // GET: api/HealthTestSchedule/test/{healthTestId}
        [HttpGet("test/{healthTestId}")]
        public async Task<IActionResult> GetByTestId(string healthTestId)
        {
            var result = await _healthTestScheduleService.GetSchedulesByTestIdAsync(healthTestId);
            return Ok(result);
        }

        // GET: api/HealthTestSchedule/available-slots?healthTestId={id}&date={date}
        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] string healthTestId, [FromQuery] DateTime date)
        {
            var result = await _healthTestScheduleService.GetAvailableSlotsAsync(healthTestId, date);
            return Ok(result);
        }

        // PUT: api/HealthTestSchedule/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] HealthTestScheduleRequestModel model)
        {
            var success = await _healthTestScheduleService.UpdateScheduleAsync(id, model);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/HealthTestSchedule/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _healthTestScheduleService.DeleteScheduleAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}