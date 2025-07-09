using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.QAThreadModel
{
    public class AnswerQuestionDto
    {
        public string Answer { get; set; }
        public string ConsultantId { get; set; }   // để xác thực đúng BS trả lời
    }
}
