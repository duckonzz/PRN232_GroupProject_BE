using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.ConsultationModels;
using GenderHealthCare.ModelViews.QueryObjects;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IConsultationsService
    {
        Task ConfirmConsultationAsync(string consultantId, string consultationId);
        Task UpdateConsultationResultAsync(string consultantId, string consultationId, string result);
        Task<ConsultationResponse> CreateBookingConsultationAsync(string userId, ConsultationRequest request);
        Task<BasePaginatedList<ConsultationResponse>> GetPagedConsultationsAsync(string consultantId, ConsultationQueryObject query);
    }
}
