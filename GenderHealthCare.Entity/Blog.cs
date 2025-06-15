using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }

        public User Author { get; set; }
    }
}
