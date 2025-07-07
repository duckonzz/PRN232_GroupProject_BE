using Microsoft.AspNetCore.Http;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Entity;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Infrastructure
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public UserContextService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetConsultantIdAsync()
        {
            var userId = GetUserId();

            var consultant = await _unitOfWork.GetRepository<Consultant>().Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.DeletedTime.HasValue);

            if (consultant == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Consultant not found for current user");
            }

            return consultant.Id;
        }

        public string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
                throw new UnauthorizedException("User is not authenticated.");

            var id = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return id ?? throw new UnauthorizedException("User ID not found in token.");
        }

        public string? GetUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }
    }
}
