using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.ModelViews.BlogModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GenderHealthCare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // POST: api/blogs
        [HttpPost]
        [Authorize] // Nếu cần xác thực
        public async Task<IActionResult> Create([FromBody] BlogRequestModel model)
        {
            // Lấy AuthorId từ token (nếu có xác thực)
            var authorId = User.FindFirstValue("id");
            if (string.IsNullOrEmpty(authorId))
                return Unauthorized();

            var result = await _blogService.CreateBlogAsync(model, authorId);

            return Ok(result);
        }

        // GET: api/blogs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _blogService.GetBlogByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/blogs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _blogService.GetAllBlogsAsync();
            return Ok(result);
        }

        // PUT: api/blogs/{id}
        [HttpPut("{id}")]
        [Authorize] // Nếu cần xác thực
        public async Task<IActionResult> Update(string id, [FromBody] BlogRequestModel model)
        {
            var success = await _blogService.UpdateBlogAsync(id, model);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/blogs/{id}
        [HttpDelete("{id}")]
        [Authorize] // Nếu cần xác thực
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _blogService.DeleteBlogAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
