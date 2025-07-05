using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.ConsultantScheduleModel;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultantSchedulesController : ControllerBase
    {
        private readonly IConsultantScheduleService _service;
        public ConsultantSchedulesController(IConsultantScheduleService service) =>
            _service = service;

        /* ---------------- GET ALL (paginated) ---------------- */
        // GET: api/ConsultantSchedules?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(BaseResponseModel<BasePaginatedList<ConsultantScheduleDto>>
                      .OkDataResponse(result, "Schedule list retrieved successfully"));
        }

        /* ---------------- SEARCH ---------------- */
        // GET: api/ConsultantSchedules/search?...params...
        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] DateTime? availableDate,
            [FromQuery] TimeSpan? startTime,
            [FromQuery] TimeSpan? endTime,
            [FromQuery] string? consultantId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.SearchAsync(
                availableDate, startTime, endTime, consultantId, page, pageSize);

            return Ok(BaseResponseModel<BasePaginatedList<ConsultantScheduleDto>>
                      .OkDataResponse(result, "Schedule search completed successfully"));
        }

        /* ---------------- GET BY ID ---------------- */
        // GET: api/ConsultantSchedules/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null)
                return NotFound(BaseResponseModel<string>.BadRequestResponse("Schedule not found"));

            return Ok(BaseResponseModel<ConsultantScheduleDto>
                      .OkDataResponse(dto, "Schedule retrieved successfully"));
        }

        /* ---------------- CREATE ---------------- */
        // POST: api/ConsultantSchedules
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConsultantScheduleDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(BaseResponseModel<string>
                      .OkDataResponse(id, "Schedule created successfully"));
        }

        /* ---------------- UPDATE ---------------- */
        // PUT: api/ConsultantSchedules/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            string id, [FromBody] UpdateConsultantScheduleDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(BaseResponse.OkMessageResponse("Schedule updated successfully"));
        }

        /* ---------------- DELETE ---------------- */
        // DELETE: api/ConsultantSchedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Schedule deleted successfully"));
        }
    }
}
