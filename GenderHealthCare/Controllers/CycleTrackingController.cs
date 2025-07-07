using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.CycleTrackingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CycleTrackingController : ControllerBase
    {
        private readonly ICycleTrackingService _cycleTrackingService;
        private readonly IUserContextService _userContext;

        public CycleTrackingController(ICycleTrackingService cycleTrackingService, IUserContextService userContext)
        {
            _cycleTrackingService = cycleTrackingService;
            _userContext = userContext;
        }

        /// <summary>
        /// Enable or disable cycle tracking for the current user.
        /// </summary>
        /// <param name="isEnabled">True to enable tracking, false to disable it.</param>
        /// <returns>Action result indicating success.</returns>
        /// <returns></returns>
        [HttpPut("enable-tracking")]
        public async Task<IActionResult> EnableTracking([FromQuery] bool isEnabled)
        {
            var userId = _userContext.GetUserId();
            await _cycleTrackingService.UpdateUserCycleTrackingAsync(userId, isEnabled);
            return Ok(BaseResponse.OkMessageResponse(isEnabled ? "Cycle tracking enabled successfully" : "Cycle tracking disabled successfully"));
        }

        /// <summary>
        /// Create a new cycle for the current user.
        /// </summary>
        /// <param name="request">Cycle tracking request data.</param>
        /// <returns>The created cycle details.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CycleTrackingRequest request)
        {
            var userId = _userContext.GetUserId();
            var result = await _cycleTrackingService.CreateCycleAsync(userId, request);
            return Ok(BaseResponseModel<CycleTrackingResponse>.OkDataResponse(result, "Cycle created successfully"));
        }

        /// <summary>
        /// Get all cycles for the current user.
        /// </summary>
        /// <returns>List of cycles.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCycles()
        {
            var userId = _userContext.GetUserId();
            var result = await _cycleTrackingService.GetCyclesAsync(userId);
            return Ok(BaseResponseModel<IEnumerable<CycleTrackingResponse>>.OkDataResponse(result, "Cycles retrieved successfully"));
        }

        /// <summary>
        /// Get details of a specific cycle by its ID.
        /// </summary>
        /// <param name="cycleId">The ID of the cycle to retrieve.</param>
        /// <returns>Cycle details.</returns>
        [HttpGet("{cycleId}")]
        public async Task<IActionResult> GetCycleById(string cycleId)
        {
            var userId = _userContext.GetUserId();
            var result = await _cycleTrackingService.GetCycleByIdAsync(cycleId, userId);
            return Ok(BaseResponseModel<CycleTrackingResponse>.OkDataResponse(result, "Cycle retrieved successfully"));
        }

        /// <summary>
        /// Delete a specific cycle by its ID.
        /// </summary>
        /// <param name="cycleId">The ID of the cycle to delete.</param>
        /// <returns>Action result indicating success.</returns>
        /// <returns></returns>
        [HttpDelete("{cycleId}")]
        public async Task<IActionResult> Delete(string cycleId)
        {
            var userId = _userContext.GetUserId();
            await _cycleTrackingService.DeleteCycleAsync(cycleId, userId);
            return Ok(BaseResponse.OkMessageResponse("Cycle deleted successfully"));
        }


    }
}
