using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.TestBookingModel
{
    public class TestBookingDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string? ResultUrl { get; set; }
        public string SlotId { get; set; }
        public string CustomerId { get; set; }
    }
}
