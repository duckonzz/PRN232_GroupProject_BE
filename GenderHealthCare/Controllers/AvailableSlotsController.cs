using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.AvailableSlotModels;
using GenderHealthCare.ModelViews.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AvailableSlotsController : ControllerBase
    {
        private readonly IAvailableSlot _availableSlotService;
        public AvailableSlotsController(IAvailableSlot availableSlotService)
        {
            _availableSlotService = availableSlotService;
        }

        /// <summary>
        /// Get available slots with optional filters and pagination.
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination.</param>
        /// <returns>Paginated list of available slots.</returns>
        [HttpGet("slots")]
        public async Task<IActionResult> GetAllSlots([FromQuery] AvailableSlotQueryObject query)
        {
            var result = await _availableSlotService.GetPagedAvailableSlotsAsync(query);
            return Ok(BaseResponseModel<BasePaginatedList<AvailableSlotResponse>>.OkDataResponse(result, "Slots retrieved sucessfully"));
        }

        /// <summary>
        /// Get details of an available slot by its ID.
        /// </summary>
        /// <param name="slotId">The ID of the slot to retrieve.</param>
        /// <returns>Slot details if found.</returns>
        [HttpGet("{slotId}")]
        public async Task<IActionResult> GetAvailableSlotById(string slotId)
        {
            var slot = await _availableSlotService.GetAvailableSlotByIdAsync(slotId);
            return Ok(BaseResponseModel<AvailableSlotResponse>.OkDataResponse(slot, "Slot retrieved successfully"));
        }
    }
}
