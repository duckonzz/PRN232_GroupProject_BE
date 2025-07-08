using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.HealthTestModels
{
    public class HealthTestResponseModel : HealthTestRequestModel
    {
        public string Id { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
