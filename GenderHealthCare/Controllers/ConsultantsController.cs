using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.ConsultantModels;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultantsController : ControllerBase
    {
        private readonly IConsultantService _service;
        public ConsultantsController(IConsultantService service) => _service = service;

        /* ---------------- GET ALL (paginated) ---------------- */
        // GET: api/Consultants?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            ServiceResponse<PaginatedList<ConsultantDto>> result =
                await _service.GetAllAsync(page, pageSize);

            return result.Success
                ? Ok(BaseResponseModel<BasePaginatedList<ConsultantDto>>
                     .OkDataResponse(result.Data, result.Message))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- SEARCH ---------------- */
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

            return result.Success
                ? Ok(BaseResponseModel<BasePaginatedList<ConsultantDto>>
                     .OkDataResponse(result.Data, result.Message))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- GET BY ID ---------------- */
        // GET: api/Consultants/{id}
        [HttpGet("{id}", Name = "GetConsultantById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _service.GetByIdAsync(id);

            return result.Success
                ? Ok(BaseResponseModel<ConsultantDto>
                     .OkDataResponse(result.Data, result.Message))
                : NotFound(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- CREATE ---------------- */
        // POST: api/Consultants
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConsultantDto dto)
        {
            var result = await _service.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(BaseResponseModel<string>
                                  .BadRequestResponse(result.Message));

            // 201 Created + Location header
            return CreatedAtAction(nameof(GetByIdAsync),
                                   new { id = result.Data },
                                   BaseResponseModel<string>.OkDataResponse(result.Data, result.Message));
        }

        /* ---------------- UPDATE ---------------- */
        // PUT: api/Consultants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            string id, [FromBody] UpdateConsultantDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            return result.Success
                ? Ok(BaseResponse.OkMessageResponse(result.Message))
                : BadRequest(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        /* ---------------- DELETE ---------------- */
        // DELETE: api/Consultants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await _service.DeleteAsync(id);

            return result.Success
                ? Ok(BaseResponse.OkMessageResponse(result.Message))
                : NotFound(BaseResponseModel<string>
                     .BadRequestResponse(result.Message));
        }

        // POST: api/Consultants/byUser/{userId}
        [HttpPost("byUser/{userId}")]
        public async Task<IActionResult> CreateFromUserAsync(
        string userId,
        [FromBody] CreateConsultantProfileDto dto)
        {
            var result = await _service.CreateFromUserAsync(userId, dto);

            if (!result.Success)
                return BadRequest(BaseResponseModel<string>
                                  .BadRequestResponse(result.Message));

            return CreatedAtRoute(
                routeName: "GetConsultantById",
                routeValues: new { id = result.Data },
                value: BaseResponseModel<string>.OkDataResponse(result.Data, result.Message));
        }
    }
}