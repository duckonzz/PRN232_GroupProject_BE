using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.AvailableSlotModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class AvailableSlotMapper
    {
        public static AvailableSlotResponse ToAvailableSlotDto(this AvailableSlot slot)
        {
            return new AvailableSlotResponse
            {
                Id = slot.Id,
                SlotStart = slot.SlotStart,
                SlotEnd = slot.SlotEnd,
                IsBooked = slot.IsBooked,
                BookedAt = slot.BookedAt,
                ScheduleId = slot.ScheduleId,
                AvailableDate = slot.Schedule.AvailableDate.Date,
                ConsultantId = slot.Schedule.ConsultantId
            };
        }

        public static List<AvailableSlotResponse> ToAvailableSlotDtoList(this IEnumerable<AvailableSlot> slots)
        {
            return slots.Select(s => s.ToAvailableSlotDto()).ToList();
        }
    }
}
