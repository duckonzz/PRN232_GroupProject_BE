using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.QAThreadModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class QAThreadService : IQAThreadService
    {
        private readonly IQAThreadRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IConsultantRepository _consultRepo;
        private readonly IMapper _mapper;

        public QAThreadService(
            IQAThreadRepository repo,
            IUserRepository userRepo,
            IConsultantRepository consultRepo,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _consultRepo = consultRepo;
            _mapper = mapper;
        }

        /* ---------- Helpers ---------- */
        private IQueryable<QAThread> BaseQuery() =>
            _repo.Query()
                 .Include(t => t.Customer)
                 .Include(t => t.Consultant)
                     .ThenInclude(c => c.User);

        /* =========================================================
           CREATE QUESTION
        ========================================================= */
        public async Task<ServiceResponse<string>> CreateQuestionAsync(CreateQuestionDto dto)
        {
            /* Kiểm tra tồn tại User & Consultant */
            if (await _userRepo.GetByIdAsync(dto.CustomerId) is null)
                return new ServiceResponse<string> { Success = false, Message = "Customer not found." };

            if (await _consultRepo.GetByIdAsync(dto.ConsultantId) is null)
                return new ServiceResponse<string> { Success = false, Message = "Consultant not found." };

            var entity = _mapper.Map<QAThread>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Data = entity.Id,
                Success = true,
                Message = "Question created."
            };
        }

        /* =========================================================
           UPDATE QUESTION (chỉ Customer sửa câu hỏi)
        ========================================================= */
        public async Task<ServiceResponse<bool>> UpdateQuestionAsync(string id, UpdateQuestionDto dto)
        {
            var thread = await _repo.GetByIdAsync(id);
            if (thread is null)
                return new ServiceResponse<bool> { Success = false, Message = "Thread not found." };

            if (thread.Answer != null)
                return new ServiceResponse<bool> { Success = false, Message = "Cannot edit after answered." };

            thread.Question = dto.Question;
            _repo.Update(thread);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Success = true, Message = "Question updated." };
        }

        /* =========================================================
           ANSWER QUESTION (Consultant)
        ========================================================= */
        public async Task<ServiceResponse<bool>> AnswerQuestionAsync(string id, AnswerQuestionDto dto)
        {
            var thread = await _repo.GetByIdAsync(id);
            if (thread is null)
                return new ServiceResponse<bool> { Success = false, Message = "Thread not found." };

            if (thread.ConsultantId != dto.ConsultantId)
                return new ServiceResponse<bool> { Success = false, Message = "Forbidden: wrong consultant." };

            if (thread.Answer != null)
                return new ServiceResponse<bool> { Success = false, Message = "Thread already answered." };

            thread.Answer = dto.Answer;
            thread.AnsweredAt = DateTimeOffset.UtcNow;

            _repo.Update(thread);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Success = true, Message = "Answered." };
        }

        /* =========================================================
           DELETE
        ========================================================= */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var thread = await _repo.GetByIdAsync(id);
            if (thread is null)
                return new ServiceResponse<bool> { Success = false, Message = "Thread not found." };

            _repo.Delete(thread);
            await _repo.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Success = true, Message = "Thread deleted." };
        }

        /* =========================================================
           GET BY ID
        ========================================================= */
        public async Task<ServiceResponse<QAThreadDto>> GetByIdAsync(string id)
        {
            var thread = await BaseQuery().FirstOrDefaultAsync(t => t.Id == id);

            return thread is null
                ? new ServiceResponse<QAThreadDto> { Success = false, Message = "Thread not found." }
                : new ServiceResponse<QAThreadDto>
                {
                    Data = _mapper.Map<QAThreadDto>(thread),
                    Success = true,
                    Message = "Retrieved."
                };
        }

        /* =========================================================
           GET ALL (Paginated)
        ========================================================= */
        public async Task<ServiceResponse<PaginatedList<QAThreadDto>>> GetAllAsync(int page, int size)
        {
            var q = BaseQuery().OrderByDescending(t => t.CreatedTime);
            var paged = await PaginatedList<QAThread>.CreateAsync(q, page, size);

            var dto = paged.Items.Select(_mapper.Map<QAThreadDto>).ToList();
            var result = new PaginatedList<QAThreadDto>(dto, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<QAThreadDto>>
            {
                Data = result,
                Success = true,
                Message = "List retrieved."
            };
        }

        /* =========================================================
           SEARCH (Paginated)
        ========================================================= */
        public async Task<ServiceResponse<PaginatedList<QAThreadDto>>> SearchAsync(
            string? customerId,
            string? consultantId,
            bool? answered,
            int page,
            int size)
        {
            var paged = await _repo.SearchAsync(customerId, consultantId, answered, page, size);
            var dto = paged.Items.Select(_mapper.Map<QAThreadDto>).ToList();
            var result = new PaginatedList<QAThreadDto>(dto, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<QAThreadDto>>
            {
                Data = result,
                Success = true,
                Message = "Search completed."
            };
        }
    }
}
