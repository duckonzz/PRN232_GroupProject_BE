using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.BlogModels;
using GenderHealthCare.Services.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BlogResponseModel> CreateBlogAsync(BlogRequestModel model, string authorId)
        {
            var blog = new Blog
            {
                Id = Guid.NewGuid().ToString(),
                Title = model.Title,
                Headline = model.Headline,
                Summary = model.Summary,
                ThumbnailUrl = model.ThumbnailUrl,
                AuthorId = authorId,
                CreatedTime = DateTimeOffset.UtcNow,
                Contents = model.Contents.Select(c => new BlogContent
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = c.Title,
                    Detail = c.Detail
                }).ToList()
            };

            await _unitOfWork.GetRepository<Blog>().InsertAsync(blog);
            await _unitOfWork.SaveAsync();

            return blog.ToBlogDto(); // Phải bao gồm cả mapping `Contents`
        }

        public async Task<BlogResponseModel?> GetBlogByIdAsync(string id)
        {
            var blog = await _unitOfWork.GetRepository<Blog>()
                .Entities
                .Include(b => b.Contents)
                .FirstOrDefaultAsync(b => b.Id == id);

            return blog?.ToBlogDto();
        }

        public async Task<List<BlogResponseModel>> GetAllBlogsAsync()
        {
            var blogs = await _unitOfWork.GetRepository<Blog>()
                .Entities
                .Include(b => b.Contents)
                .ToListAsync();

            return blogs.Select(b => b.ToBlogDto()).OrderByDescending(b => b.CreatedTime).ToList();
        }

        public async Task<bool> UpdateBlogAsync(string id, BlogRequestModel model)
        {
            var blogRepo = _unitOfWork.GetRepository<Blog>();
            var blogContentRepo = _unitOfWork.GetRepository<BlogContent>();

            var blog = await blogRepo.Entities
                .Include(b => b.Contents)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return false;

            // Update Blog info
            blog.Title = model.Title;
            blog.Headline = model.Headline;
            blog.Summary = model.Summary;
            blog.ThumbnailUrl = model.ThumbnailUrl;

            // Xóa các nội dung cũ
            foreach (var content in blog.Contents)
            {
                blogContentRepo.Delete(content.Id);
            }

            // Thêm nội dung mới
            blog.Contents = model.Contents.Select(c => new BlogContent
            {
                Id = Guid.NewGuid().ToString(),
                Title = c.Title,
                Detail = c.Detail,
                BlogId = blog.Id
            }).ToList();

            blogRepo.Update(blog);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteBlogAsync(string id)
        {
            var blogRepo = _unitOfWork.GetRepository<Blog>();
            var blog = await blogRepo.GetByIdAsync(id);
            if (blog == null) return false;

            blogRepo.Delete(id);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
