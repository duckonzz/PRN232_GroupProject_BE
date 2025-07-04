using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.TestBookingModel
{
    public class UpdateTestBookingDto
    {
        public string Status { get; set; }               // Pending / Completed / Cancelled
        public string? ResultUrl { get; set; }           // link kết quả xét nghiệm (nếu có)
        public string SlotId { get; set; }               // NEW – cho phép chuyển sang slot khác
    }

}
