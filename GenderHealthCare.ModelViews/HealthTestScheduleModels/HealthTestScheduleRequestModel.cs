using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.ModelViews.HealthTestScheduleModels
{
    public class HealthTestScheduleRequestModel
    {
        public string HealthTestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public int SlotDurationInMinutes { get; set; }
        public List<string> DaysOfWeek { get; set; }
    }
}
