using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.ConsultantModel;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultantsController : ControllerBase
    {
        private readonly IConsultantService _service;
        public ConsultantsController(IConsultantService service) => _service = service;

        /* ---------- GET ALL ---------- */
        // GET: api/Consultants?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(BaseResponseModel<BasePaginatedList<ConsultantDto>>
                      .OkDataResponse(result, "Consultant list retrieved successfully"));
        }

        /* ---------- SEARCH ---------- */
        // GET: api/Consultants/search?...params...
        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string? degree,
            [FromQuery] string? email,
            [FromQuery] int? expYears,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.SearchAsync(degree, email, expYears, page, pageSize);
            return Ok(BaseResponseModel<BasePaginatedList<ConsultantDto>>
                      .OkDataResponse(result, "Consultant search completed successfully"));
        }

        /* ---------- GET BY ID ---------- */
        // GET: api/Consultants/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null)
                return NotFound(BaseResponseModel<string>.BadRequestResponse("Consultant not found"));

            return Ok(BaseResponseModel<ConsultantDto>
                      .OkDataResponse(dto, "Consultant retrieved successfully"));
        }

        /* ---------- CREATE ---------- */
        // POST: api/Consultants
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConsultantDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(BaseResponseModel<string>
                      .OkDataResponse(id, "Consultant created successfully"));
        }

        /* ---------- UPDATE ---------- */
        // PUT: api/Consultants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            string id, [FromBody] UpdateConsultantDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(BaseResponse.OkMessageResponse("Consultant updated successfully"));
        }

        /* ---------- DELETE ---------- */
        // DELETE: api/Consultants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Consultant deleted successfully"));
        }
    }
}
