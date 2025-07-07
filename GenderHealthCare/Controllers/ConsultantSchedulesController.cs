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

            return result.Success
                ? Ok(BaseResponseModel<BasePaginatedList<ConsultantScheduleDto>>
                     .OkDataResponse(result.Data, "Schedule list retrieved successfully"))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
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

            return result.Success
                ? Ok(BaseResponseModel<BasePaginatedList<ConsultantScheduleDto>>
                     .OkDataResponse(result.Data, "Schedule search completed successfully"))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- GET BY ID ---------------- */
        // GET: api/ConsultantSchedules/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _service.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(BaseResponseModel<string>.BadRequestResponse(result.Message));

            return Ok(BaseResponseModel<ConsultantScheduleDto>
                      .OkDataResponse(result.Data, "Schedule retrieved successfully"));
        }

        /* ---------------- CREATE ---------------- */
        // POST: api/ConsultantSchedules
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConsultantScheduleDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return result.Success
                ? Ok(BaseResponseModel<string>
                     .OkDataResponse(result.Data, "Schedule created successfully"))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- UPDATE ---------------- */
        // PUT: api/ConsultantSchedules/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            string id, [FromBody] UpdateConsultantScheduleDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            return result.Success
                ? Ok(BaseResponse.OkMessageResponse("Schedule updated successfully"))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- DELETE ---------------- */
        // DELETE: api/ConsultantSchedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await _service.DeleteAsync(id);

            return result.Success
                ? Ok(BaseResponse.OkMessageResponse("Schedule deleted successfully"))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }
    }
}
