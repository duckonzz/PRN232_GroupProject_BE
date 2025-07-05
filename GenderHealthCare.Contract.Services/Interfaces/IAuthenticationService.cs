using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.AuthenticationModels;
using GenderHealthCare.ModelViews.QueryObjects;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserResponseModel> GetInfoAsync(string userId);
        Task<BasePaginatedList<UserResponseModel>> GetPagedUsersAsync(UserQueryObject query);
        Task<UserResponseModel> GetUserByIdAsync(string id);
        Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
        // Register & Login (Password)
        Task<UserResponseModel> RegisterAsync(UserRegistrationRequest request);
        Task<AuthenticationModel> LoginWithEmailPasswordAsync(LoginRequest request);
        Task<UserResponseModel> UpdateUserAsync(string id, UpdateUserRequest request);
        Task DeleteUserAsync(string id);
        // Admin-only: update role
        Task<UserResponseModel> UpdateUserRoleAsync(string userId, Role newRole);
        Task ApproveConsultantAsync(string userId);
        Task RejectConsultantAsync(string userId);
    }
}
