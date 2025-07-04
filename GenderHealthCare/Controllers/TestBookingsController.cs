using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.TestBookingModel;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestBookingsController : ControllerBase
    {
        private readonly ITestBookingService _service;

        public TestBookingsController(ITestBookingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetAllAsync(page, size);
            return Ok(BaseResponseModel<BasePaginatedList<TestBookingDto>>
                      .OkDataResponse(result, "Test bookings retrieved successfully"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string? status,
            [FromQuery] string? customerId,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _service.SearchAsync(status, customerId, page, size);
            return Ok(BaseResponseModel<BasePaginatedList<TestBookingDto>>
                      .OkDataResponse(result, "Search completed successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null)
                return NotFound(BaseResponseModel<string>.BadRequestResponse("Test booking not found"));

            return Ok(BaseResponseModel<TestBookingDto>.OkDataResponse(dto, "Retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTestBookingDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(BaseResponseModel<string>.OkDataResponse(id, "Created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateTestBookingDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }

}
