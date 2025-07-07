using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.FeedbackModels
{
    public class CreateFeedbackDto
    {
        public string TargetType { get; set; }   // "Consultation", "HealthTest", etc.
        public string TargetId { get; set; }
        public int Rating { get; set; }   // 1‑5
        public string? Comment { get; set; }
        public string UserId { get; set; }
    }
}
