using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.BlogModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class BlogMapper
    {
        public static BlogResponseModel ToBlogDto(this Blog blog)
        {
            return new BlogResponseModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Headline = blog.Headline,
                Summary = blog.Summary,
                ThumbnailUrl = blog.ThumbnailUrl,
                AuthorId = blog.AuthorId,
                CreatedTime = blog.CreatedTime,

                Contents = blog.Contents?.Select(c => new BlogContentResponse
                {
                    Title = c.Title,
                    Detail = c.Detail
                }).ToList() ?? new List<BlogContentResponse>()
            };
        }
    }
}
