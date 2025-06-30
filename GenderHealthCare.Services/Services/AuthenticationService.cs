using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Config;
using GenderHealthCare.Core.Constants;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Exceptions;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.Core.Models;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.AuthenticationModels;
using GenderHealthCare.ModelViews.QueryObjects;
using GenderHealthCare.Services.Infrastructure;
using GenderHealthCare.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtSettings _jwtSettings;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthenticationService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, JwtSettings jwtSettings, JwtTokenGenerator tokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtSettings;
            _tokenGenerator = tokenGenerator;
        }
        public async Task DeleteUserAsync(string id)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            User user = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == id && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            user.DeletedTime = CoreHelper.SystemTimeNow;

            userRepo.Update(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task<BasePaginatedList<UserResponseModel>> GetPagedUsersAsync(UserQueryObject query)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var usersQuery = userRepo.Entities
                               .AsNoTracking()
                               .Where(u => !u.DeletedTime.HasValue);

            if (!string.IsNullOrWhiteSpace(query.Id))
                usersQuery = usersQuery.Where(u => u.Id == query.Id);

            if (!string.IsNullOrWhiteSpace(query.FullName))
                usersQuery = usersQuery.Where(u => u.FullName.Contains(query.FullName));

            if (!string.IsNullOrWhiteSpace(query.Email))
                usersQuery = usersQuery.Where(u => u.Email.Contains(query.Email));

            if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
                usersQuery = usersQuery.Where(u => u.PhoneNumber.Contains(query.PhoneNumber));

            if (query.IsCycleTrackingOn.HasValue)
                usersQuery = usersQuery.Where(u => u.IsCycleTrackingOn == query.IsCycleTrackingOn.Value);

            if (query.Role.HasValue)
                usersQuery = usersQuery.Where(u => u.Role == query.Role.Value.ToString());

            // Sorting logic
            usersQuery = !string.IsNullOrWhiteSpace(query.SortBy) ? query.SortBy.ToLower() switch
            {
                "fullname" => query.IsDescending ? usersQuery.OrderByDescending(u => u.FullName) : usersQuery.OrderBy(u => u.FullName),
                "email" => query.IsDescending ? usersQuery.OrderByDescending(u => u.Email) : usersQuery.OrderBy(u => u.Email),
                "phonenumber" => query.IsDescending ? usersQuery.OrderByDescending(u => u.PhoneNumber) : usersQuery.OrderBy(u => u.PhoneNumber),
                _ => usersQuery.OrderByDescending(u => u.CreatedTime)
            } : usersQuery.OrderByDescending(u => u.CreatedTime);

            // Paging
            int totalCount = await usersQuery.CountAsync();
            var pagedUsers = await usersQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var dto = pagedUsers.ToListUserDto();
            return new BasePaginatedList<UserResponseModel>(dto, totalCount, query.PageIndex, query.PageSize);
        }

        public async Task<UserResponseModel> GetUserByIdAsync(string id)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            User user = await userRepo.Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            return user.ToUserDto();
        }

        public async Task<UserResponseModel> GetInfoAsync(string userId)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            User user = await userRepo.Entities.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            return user.ToUserDto();
        }

        public async Task<AuthenticationModel> LoginWithEmailPasswordAsync(LoginRequest request)
        {
            var normalizedEmail = NormalizeEmail(request.Email);

            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Invalid email or password");
            }

            if (user.DeletedTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "User account is deleted");
            }

            return await _tokenGenerator.CreateToken(user, _jwtSettings);
        }

        public async Task<UserResponseModel> RegisterAsync(UserRegistrationRequest request)
        {
            var nomalizedEmail = NormalizeEmail(request.Email);

            var userRepo = _unitOfWork.GetRepository<User>();
            var exists = await userRepo.Entities.AsNoTracking().AnyAsync(u => u.Email == nomalizedEmail);

            if (exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.DUPLICATE , "Email is already registered");
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = nomalizedEmail,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender.ToString(),
                Role = Role.Customer.ToString(),
            };

            await userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            return user.ToUserDto();
        }

        public async Task SetPasswordAsync(string userId, SetPasswordRequest request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            if (request.Password != request.ConfirmPassword)
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Passwords do not match");

            if (!PasswordHelper.IsStrongPassword(request.Password))
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Password must be at least 8 characters and include uppercase, lowercase, number and special character.");

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            userRepo.Update(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task<UserResponseModel> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            User user = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == id && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            var updatedDateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            var updatedGender = request.Gender?.ToString() ?? user.Gender;
            var updatedIsCycleTrackingOn = request.IsCycleTrackingOn ?? user.IsCycleTrackingOn;

            // Update user properties
            user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName;
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? user.PhoneNumber : request.PhoneNumber;
            user.DateOfBirth = updatedDateOfBirth;
            user.Gender = updatedGender;
            user.IsCycleTrackingOn = updatedIsCycleTrackingOn;

            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            userRepo.Update(user);
            await _unitOfWork.SaveAsync();

            return user.ToUserDto();
        }

        public async Task<UserResponseModel> UpdateUserRoleAsync(string userId, Role newRole)
        {
            if (newRole == Role.Admin)
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You cannot assign Admin role via this API.");

            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            user.Role = newRole.ToString();
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            userRepo.Update(user);
            await _unitOfWork.SaveAsync();

            return user.ToUserDto();
        }

        #region shared private methods
        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
        #endregion
    }
}
