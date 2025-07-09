using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.TestSlotModels
{
    public class CreateTestSlotResultDto
    {
        public string SlotId { get; set; } = default!;
        public string? BookingId { get; set; }
    }
}
