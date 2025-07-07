using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.ConsultantModel
{
    public class CreateConsultantProfileDto
    {
        public string Degree { get; set; } = default!;
        public int Experience { get; set; }           // years
        public string Bio { get; set; } = default!;
    }
}
