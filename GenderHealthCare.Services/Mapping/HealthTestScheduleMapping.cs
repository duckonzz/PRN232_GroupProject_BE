using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.HealthTestScheduleModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class HealthTestScheduleMapping
    {
        public static HealthTestScheduleResponseModel ToHealthTestScheduleDto(this HealthTestSchedule schedule)
        {
            if (schedule == null)
                return null;

            return new HealthTestScheduleResponseModel
            {
                Id = schedule.Id,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                SlotStart = schedule.SlotStart,
                SlotEnd = schedule.SlotEnd,
                SlotDurationInMinutes = schedule.SlotDurationInMinutes,
                DaysOfWeek = !string.IsNullOrEmpty(schedule.DaysOfWeek)
                    ? schedule.DaysOfWeek.Split(',').ToList()
                    : new List<string>(),
                HealthTestId = schedule.HealthTestId,
                CreatedTime = schedule.CreatedTime
            };
        }
    }
}
