using GenderHealthCare.Contract.Services.Interfaces;
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

        // GET: api/consultants?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _service.GetAllAsync(page, pageSize));

        // GET: api/consultants/search?degree=Ob&email=@mail&expYears=5&page=1&pageSize=10
        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string? degree,
            [FromQuery] string? email,
            [FromQuery] int? expYears,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
            => Ok(await _service.SearchAsync(degree, email, expYears, page, pageSize));

        // GET: api/consultants/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
            => (await _service.GetByIdAsync(id)) is { } dto ? Ok(dto) : NotFound();

        // POST: api/consultants
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConsultantDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id }, null);
        }

        // PUT: api/consultants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateConsultantDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/consultants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
