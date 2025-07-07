using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.BlogModels
{
    public class BlogRequestModel
    {
        public string Title { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? ThumbnailUrl { get; set; }


        public List<BlogContentRequestModel> Contents { get; set; } = new List<BlogContentRequestModel>();
    }
}
