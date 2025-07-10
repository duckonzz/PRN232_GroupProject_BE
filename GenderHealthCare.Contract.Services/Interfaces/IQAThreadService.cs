using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.QAThreadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IQAThreadService
    {
        Task<ServiceResponse<string>> CreateQuestionAsync(CreateQuestionDto dto);
        Task<ServiceResponse<bool>> UpdateQuestionAsync(string id, UpdateQuestionDto dto);
        Task<ServiceResponse<bool>> AnswerQuestionAsync(string id, AnswerQuestionDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string id);

        Task<ServiceResponse<QAThreadDto>> GetByIdAsync(string id);
        Task<ServiceResponse<PaginatedList<QAThreadDto>>> GetAllAsync(int page, int size);

        Task<ServiceResponse<PaginatedList<QAThreadDto>>> SearchAsync(
            string? customerId,
            bool? answered,
            int page,
            int size);

        Task<ServiceResponse<PaginatedList<QAThreadHistoryDto>>> GetConversationAsync(
            string customerId,
            int page,
            int size);  
    }
}
