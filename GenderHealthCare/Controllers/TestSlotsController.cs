using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.TestSlotModels;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestSlotsController : ControllerBase
    {
        private readonly ITestSlotService _service;
        public TestSlotsController(ITestSlotService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetAllAsync(page, size);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] DateTime? testDate,
            [FromQuery] string? userId,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _service.SearchAsync(testDate, userId, page, size);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}", Name = "GetTestSlotById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{id}/booking")]
        public async Task<IActionResult> UpdateBookingAsync(string id, [FromBody] UpdateTestSlotBookingDto dto)
        {
            var result = await _service.UpdateByIdAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateTestSlotDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserAsync(
        string userId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
        {
            var result = await _service.GetByUserAsync(userId, page, size);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPatch("{id}/updateStatus")]
        public async Task<IActionResult> MarkAsBookedAsync(string id)
        {
            var result = await _service.UpdateStatus(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
