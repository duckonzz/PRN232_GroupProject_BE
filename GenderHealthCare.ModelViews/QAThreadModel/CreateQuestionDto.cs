using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.QAThreadModel
{
    public class CreateQuestionDto
    {
        public string Question { get; set; }
        public string ConsultantId { get; set; }   // chọn BS cần hỏi
        public string CustomerId { get; set; }
    }
}
