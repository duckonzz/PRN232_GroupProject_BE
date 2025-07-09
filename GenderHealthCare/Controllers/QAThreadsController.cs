using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.QAThreadModel;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QAThreadsController : ControllerBase
    {
        private readonly IQAThreadService _service;
        public QAThreadsController(IQAThreadService service) => _service = service;

        /* ---------- CREATE ---------- */
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
        {
            var res = await _service.CreateQuestionAsync(dto);
            if (!res.Success) return BadRequest(res);
            return CreatedAtRoute("GetQAThreadById", new { id = res.Data }, res);
        }

        /* ---------- READ ---------- */
        [HttpGet("{id}", Name = "GetQAThreadById")]
        public async Task<IActionResult> GetById(string id)
        {
            var res = await _service.GetByIdAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var res = await _service.GetAllAsync(page, size);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? customerId,
            [FromQuery] string? consultantId,
            [FromQuery] bool? answered,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var res = await _service.SearchAsync(customerId, consultantId, answered, page, size);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        /* ---------- UPDATE ---------- */
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(string id, [FromBody] UpdateQuestionDto dto)
        {
            var res = await _service.UpdateQuestionAsync(id, dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut("{id}/answer")]
        public async Task<IActionResult> Answer(string id, [FromBody] AnswerQuestionDto dto)
        {
            var res = await _service.AnswerQuestionAsync(id, dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        /* ---------- DELETE ---------- */
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var res = await _service.DeleteAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }
    }
}
