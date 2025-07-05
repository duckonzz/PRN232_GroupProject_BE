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
        /// 
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        [HttpPut("enable-tracking")]
        public async Task<IActionResult> EnableTracking([FromQuery] bool isEnabled)
        {
            var userId = _userContext.GetUserId();
            await _cycleTrackingService.UpdateUserCycleTrackingAsync(userId, isEnabled);
            return Ok(BaseResponse.OkMessageResponse(isEnabled ? "Cycle tracking enabled successfully" : "Cycle tracking disabled successfully"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CycleTrackingRequest request)
        {
            var userId = _userContext.GetUserId();
            var result = await _cycleTrackingService.CreateCycleAsync(userId, request);
            return Ok(BaseResponseModel<CycleTrackingResponse>.OkDataResponse(result, "Cycle created successfully"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCycles()
        {
            var userId = _userContext.GetUserId();
            var result = await _cycleTrackingService.GetCyclesAsync(userId);
            return Ok(BaseResponseModel<IEnumerable<CycleTrackingResponse>>.OkDataResponse(result, "Cycles retrieved successfully"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycleId"></param>
        /// <returns></returns>
        [HttpGet("{cycleId}")]
        public async Task<IActionResult> GetCycleById(string cycleId)
        {
            var userId = _userContext.GetUserId();
            var result = await _cycleTrackingService.GetCycleByIdAsync(cycleId, userId);
            return Ok(BaseResponseModel<CycleTrackingResponse>.OkDataResponse(result, "Cycle retrieved successfully"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycleId"></param>
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
