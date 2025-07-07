using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.FeedbackModels
{
    public class UpdateFeedbackDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
