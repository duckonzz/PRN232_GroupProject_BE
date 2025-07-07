using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.FeedbackModels;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _service;
        public FeedbacksController(IFeedbackService service) => _service = service;

        /* -- GET ALL -- */
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var res = await _service.GetAllAsync(page, size);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        /* -- SEARCH -- */
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? targetType,
            [FromQuery] string? targetId,
            [FromQuery] string? userId,
            [FromQuery] int? minRating,
            [FromQuery] int? maxRating,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var res = await _service.SearchAsync(targetType, targetId, userId, minRating, maxRating, page, size);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        /* -- GET BY ID -- */
        [HttpGet("{id}", Name = "GetFeedbackById")]
        public async Task<IActionResult> GetById(string id)
        {
            var res = await _service.GetByIdAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        /* -- CREATE -- */
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDto dto)
        {
            var res = await _service.CreateAsync(dto);
            if (!res.Success) return BadRequest(res);
            return CreatedAtRoute("GetFeedbackById", new { id = res.Data }, res);
        }

        /* -- UPDATE -- */
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateFeedbackDto dto)
        {
            var res = await _service.UpdateAsync(id, dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        /* -- DELETE -- */
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var res = await _service.DeleteAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }
    }
}
