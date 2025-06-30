using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.AuthenticationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserContextService _contextService;

        public AuthenticationController(IAuthenticationService authenticationService, IUserContextService contextService)
        {
            _authenticationService = authenticationService;
            _contextService = contextService;
        }

        /// <summary>
        /// Register a new user using email and password.
        /// </summary>
        /// <param name="request">
        /// Registration information including FullName, Email, PhoneNumber, Password, ConfirmPassword, DateOfBirth, Gender, and Role.
        ///
        /// <para><b>Gender:</b> Enum value (int)</para>
        /// <para>- 0 = Male</para>
        /// <para>- 1 = Female</para>
        /// <para>- 2 = Other</para>
        ///
        /// <para><b>Role:</b> Enum value (int)</para>
        /// <para>- 1 = Customer</para>
        /// <para>- 2 = Consultant</para>
        /// <para>- 3 = Staff</para>
        /// <para>- 4 = Manager</para>
        /// <para>- 5 = Admin</para>
        ///
        /// <para><b>⚠ Note:</b> If <c>Role = Consultant</c>, the user will be registered as a <c>Customer</c> first with <c>ConsultantStatus = "Pending"</c>. Admin approval is required to activate Consultant privileges.</para>
        /// </param>
        /// <returns>User information after successful registration.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            return Ok(BaseResponseModel<UserResponseModel>.OkDataResponse(result, "User registered successfully"));
        }

        /// <summary>
        /// Login with email and password.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>JWT token and user information.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithEmailPassword([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginWithEmailPasswordAsync(request);
            return Ok(BaseResponseModel<AuthenticationModel>.OkDataResponse(result, "Login successfully"));
        }

        /// <summary>
        /// <summary>
        /// Sets a password for the authenticated user. This is required if the user logged in using Google or has not set a password yet.
        /// </summary>
        /// <param name="request">The request containing the new password details.</param>
        /// <returns>A response indicating the success or failure of the password setting operation.</returns>
        [HttpPost("set-password")]
        [Authorize]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            var userId = _contextService.GetUserId();
            await _authenticationService.SetPasswordAsync(userId, request);
            return Ok(BaseResponse.OkMessageResponse("Password set successfully"));
        }

        /// <summary>
        /// Get the current authenticated user's profile.
        /// </summary>
        /// <returns>User profile information.</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfileAsync()
        {
            var userId = _contextService.GetUserId();
            var result = await _authenticationService.GetInfoAsync(userId);
            return Ok(BaseResponseModel<UserResponseModel>.OkDataResponse(result, "User profile retrieved successfully."));
        }

        /// <summary>
        /// Update the authenticated user's own profile.
        /// </summary>
        /// <param name="request">New profile data.</param>
        /// <returns>Updated user info.</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateUserRequest request)
        {
            var userId = _contextService.GetUserId();
            var result = await _authenticationService.UpdateUserAsync(userId, request);
            return Ok(BaseResponseModel<UserResponseModel>.OkDataResponse(result, "Profile updated successfully."));
        }

    }
}
