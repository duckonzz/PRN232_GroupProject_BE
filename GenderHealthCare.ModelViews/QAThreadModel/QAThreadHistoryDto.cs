using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.QAThreadModel
{
    public class QAThreadHistoryDto
    {
        public string Id { get; set; } = default!;
        public string CustomerId { get; set; } = default!;

        public string Question { get; set; } = default!;
        public DateTimeOffset CreatedTime { get; set; }

        public string? Answer { get; set; }
        public DateTimeOffset? AnsweredAt { get; set; }
    }
}
