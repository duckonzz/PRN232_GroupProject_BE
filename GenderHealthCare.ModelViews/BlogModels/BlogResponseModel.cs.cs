using GenderHealthCare.Entity;

namespace GenderHealthCare.ModelViews.BlogModels
{
    public class BlogResponseModel
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string? Headline { get; set; }
            public string? Summary { get; set; }
            public string? ThumbnailUrl { get; set; }
            public List<BlogContentResponse> Contents { get; set; }
            public string AuthorId { get; set; }
       
            public DateTimeOffset CreatedTime { get; set; }
        }
    }
