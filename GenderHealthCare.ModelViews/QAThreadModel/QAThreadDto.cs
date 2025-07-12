using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.QAThreadModel
{
    public class QAThreadDto
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
        public DateTimeOffset? AnsweredAt { get; set; }
        public string CustomerId { get; set; }
        // thông tin hiển thị thêm
        public string CustomerName { get; set; }
    }
}
