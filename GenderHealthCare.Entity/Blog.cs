using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string AuthorId { get; set; }

        public string? Headline { get; set; }

        public string? Summary { get; set; }

        public string? ThumbnailUrl { get; set; }
        public ICollection<BlogContent> Contents { get; set; } 
        public User Author { get; set; }


    }
}
