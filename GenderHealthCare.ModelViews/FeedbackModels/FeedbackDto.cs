using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.FeedbackModels
{
    public class FeedbackDto
    {
        public string Id { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedTime { get; set; }

        /* optional shallow user info */
        public string UserId { get; set; }
        public string FullName { get; set; }
    }
}
