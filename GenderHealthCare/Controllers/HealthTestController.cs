using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.HealthTestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthTestController : ControllerBase
    {
        private readonly IHealthTestService _healthTestService;

        public HealthTestController(IHealthTestService healthTestService)
        {
            _healthTestService = healthTestService;
        }

        // POST: api/healthtest
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] HealthTestRequestModel model)
        {
            // You might want to verify admin role here if only admins can create tests
            var userId = User.FindFirstValue("id");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _healthTestService.CreateHealthTestAsync(model);
            return Ok(result);
        }

        // GET: api/healthtest/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _healthTestService.GetHealthTestByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/healthtest
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _healthTestService.GetAllHealthTestsAsync();
            return Ok(result);
        }

        // GET: api/healthtest/search?keyword={keyword}
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var result = await _healthTestService.SearchTestsAsync(keyword);
            return Ok(result);
        }

        // PUT: api/healthtest/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] HealthTestRequestModel model)
        {
            var success = await _healthTestService.UpdateHealthTestAsync(id, model);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/healthtest/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _healthTestService.DeleteHealthTestAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}