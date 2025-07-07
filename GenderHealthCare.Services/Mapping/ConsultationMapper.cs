using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.ConsultationModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class ConsultationMapper
    {
        public static ConsultationResponse ToConsultationDto(this Consultation consultation)
        {
            return new ConsultationResponse
            {
                Id = consultation.Id,
                Reason = consultation.Reason,
                Status = consultation.Status,
                Result = consultation.Result,

                SlotId = consultation.SlotId,
                AvailableDate = consultation.Slot.Schedule.AvailableDate,
                SlotStart = consultation.Slot.SlotStart,
                SlotEnd = consultation.Slot.SlotEnd,

                ConsultantId = consultation.ConsultantId,
                ConsultantFullName = consultation.Consultant.User.FullName,

                UserId = consultation.UserId,
                UserFullName = consultation.User.FullName
            };
        }

        public static List<ConsultationResponse> ToConsultationDtoList(this IEnumerable<Consultation> consultations)
        {
            return consultations.Select(c => c.ToConsultationDto()).ToList();
        }
    }
}
