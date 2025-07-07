using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class BlogContent : BaseEntity
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string BlogId { get; set; }  
        public Blog Blog { get; set; }
    }
}
