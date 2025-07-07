using AutoMapper;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Repositories.PaggingItems;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.FeedbackModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public FeedbackService(
            IFeedbackRepository repo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        private async Task<double> RecalcAvgAsync(string targetType, string targetId)
        {
            return await _repo.Query()
                              .Where(f => f.TargetType == targetType &&
                                          f.TargetId == targetId)
                              .AverageAsync(f => f.Rating);
        }

        /* ---------------------------------------------------------
   CREATE
--------------------------------------------------------- */
        public async Task<ServiceResponse<string>> CreateAsync(CreateFeedbackDto dto)
        {
            /* 0. rating range 0‑5 */
            if (dto.Rating < 0 || dto.Rating > 5)
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Rating must be between 0 and 5."
                };

            /* 1. user exists? */
            if (await _userRepo.GetByIdAsync(dto.UserId) is null)
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User not found."
                };

            /* 2. add feedback */
            var entity = _mapper.Map<Feedback>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            /* 3. new average */
            double newAvg = await RecalcAvgAsync(dto.TargetType, dto.TargetId);

            return new ServiceResponse<string>
            {
                Data = entity.Id,
                Success = true,
                Message = $"Feedback created. New average rating: {newAvg:0.##}"
            };
        }

        /* ---------------------------------------------------------
           UPDATE
        --------------------------------------------------------- */
        public async Task<ServiceResponse<bool>> UpdateAsync(string id, UpdateFeedbackDto dto)
        {
            /* 0. rating range 0‑5 */
            if (dto.Rating < 0 || dto.Rating > 5)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Rating must be between 0 and 5."
                };

            var fb = await _repo.GetByIdAsync(id);
            if (fb is null)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Feedback not found."
                };

            string tgtType = fb.TargetType;
            string tgtId = fb.TargetId;

            fb.Rating = dto.Rating;
            fb.Comment = dto.Comment;
            await _repo.SaveChangesAsync();

            double newAvg = await RecalcAvgAsync(tgtType, tgtId);

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = $"Updated. New average rating: {newAvg:0.##}"
            };
        }


        /* ---------------------------------------------------------
           DELETE
        --------------------------------------------------------- */
        public async Task<ServiceResponse<bool>> DeleteAsync(string id)
        {
            var fb = await _repo.GetByIdAsync(id);
            if (fb is null)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Feedback not found."
                };

            string tgtType = fb.TargetType;
            string tgtId = fb.TargetId;

            _repo.Delete(fb);
            await _repo.SaveChangesAsync();

            /* recalc after delete (optional) */
            double newAvg = await RecalcAvgAsync(tgtType, tgtId);

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = $"Deleted. New average rating: {newAvg:0.##}"
            };
        }

        /* ---------------------------------------------------------
           READ SINGLE
        --------------------------------------------------------- */
        public async Task<ServiceResponse<FeedbackDto>> GetByIdAsync(string id)
        {
            var fb = await _repo.GetByIdAsync(id);
            return fb is null
                ? new ServiceResponse<FeedbackDto>
                {
                    Success = false,
                    Message = "Feedback not found."
                }
                : new ServiceResponse<FeedbackDto>
                {
                    Data = _mapper.Map<FeedbackDto>(fb),
                    Success = true,
                    Message = "Retrieved."
                };
        }

        /* ---------------------------------------------------------
           READ ALL (paginated)
        --------------------------------------------------------- */
        public async Task<ServiceResponse<PaginatedList<FeedbackDto>>> GetAllAsync(int page, int size)
        {
            var paged = await PaginatedList<Feedback>
                             .CreateAsync(_repo.Query().OrderByDescending(f => f.CreatedTime), page, size);

            var dtoLst = paged.Items.Select(_mapper.Map<FeedbackDto>).ToList();
            var result = new PaginatedList<FeedbackDto>(dtoLst, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<FeedbackDto>>
            {
                Data = result,
                Success = true,
                Message = "List retrieved."
            };
        }

        /* ---------------------------------------------------------
           SEARCH (paginated)
        --------------------------------------------------------- */
        public async Task<ServiceResponse<PaginatedList<FeedbackDto>>> SearchAsync(
            string? targetType,
            string? targetId,
            string? userId,
            int? minRating,
            int? maxRating,
            int page,
            int size)
        {
            var paged = await _repo.SearchAsync(targetType, targetId, userId,
                                                 minRating, maxRating, page, size);

            var dtoLst = paged.Items.Select(_mapper.Map<FeedbackDto>).ToList();
            var result = new PaginatedList<FeedbackDto>(dtoLst, paged.TotalCount, page, size);

            return new ServiceResponse<PaginatedList<FeedbackDto>>
            {
                Data = result,
                Success = true,
                Message = "Search completed."
            };
        }
    }
}
