using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.FeedbackModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<ServiceResponse<string>> CreateAsync(CreateFeedbackDto dto);
        Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateFeedbackDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        Task<ServiceResponse<FeedbackDto>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<FeedbackDto>>> GetAllAsync(int page, int size);
        Task<ServiceResponse<PaginatedList<FeedbackDto>>> SearchAsync(
            string? targetType, string? targetId, string? userId,
            int? minRating, int? maxRating, int page, int size);
    }
}
