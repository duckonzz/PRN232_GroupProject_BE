using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.ConsultationModels;
using GenderHealthCare.ModelViews.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IUserContextService _userContextService;
        private readonly IConsultationsService _consultationService;

        public ConsultationController(IUserContextService userContextService, IConsultationsService consultationService)
        {
            _userContextService = userContextService;
            _consultationService = consultationService;
        }

        /// <summary>
        /// Book a consultation with a consultant
        /// </summary>
        /// <param name="request">The request containing reason and slot ID</param>
        /// <returns>Details of the booked consultation</returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateBookingConsultation([FromBody] ConsultationRequest request)
        {
            var userId = _userContextService.GetUserId();
            var result = await _consultationService.CreateBookingConsultationAsync(userId, request);

            return Ok(BaseResponseModel<ConsultationResponse>.OkDataResponse(result, "Created booking consultation successfully"));
        }

        /// <summary>
        /// Get a paginated list of consultations for the logged-in consultant
        /// </summary>
        /// <param name="query">
        /// Filter and pagination options:
        /// <br/>• <b>Status</b>: (optional) Filter by consultation status. Acceptable values:
        /// <list type="bullet">
        /// <item><description><b>Pending</b> (0): User has booked the consultation, awaiting confirmation</description></item>
        /// <item><description><b>Confirmed</b> (1): Consultant has confirmed the consultation</description></item>
        /// <item><description><b>Completed</b> (2): Consultation has been completed and a result is available</description></item>
        /// <item><description><b>Cancelled</b> (3): Consultation was cancelled by user or consultant</description></item>
        /// </list>
        /// <br/>• <b>FromDate</b>, <b>ToDate</b>: (optional) Filter consultations between two dates (by AvailableDate)
        /// <br/>• <b>SortBy</b>: (optional) Field to sort by. Acceptable values: <c>date</c>, <c>status</c>, <c>starttime</c>
        /// <br/>• <b>IsDescending</b>: (default = true) Whether to sort in descending order
        /// <br/>• <b>PageIndex</b>, <b>PageSize</b>: (default = 1, 10) Pagination parameters
        /// </param>
        /// <returns>A paginated list of consultations</returns>
        [Authorize(Roles = "Consultant")]
        [HttpGet("consultations")]
        public async Task<IActionResult> GetConsultations([FromQuery] ConsultationQueryObject query)
        {
            var consultantId = await _userContextService.GetConsultantIdAsync();
            var result = await _consultationService.GetPagedConsultationsAsync(consultantId, query);

            return Ok(BaseResponseModel<BasePaginatedList<ConsultationResponse>>.OkDataResponse(result, "Retrieved consultations successfully"));
        }

        /// <summary>
        /// Confirm a consultation that has been booked by a user.
        /// </summary>
        /// <param name="id">The ID of the consultation to confirm.</param>
        /// <returns>Success message if the consultation was confirmed.</returns>
        [Authorize(Roles = "Consultant")]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmConsultation(string id)
        {
            var consultantId = await _userContextService.GetConsultantIdAsync();
            await _consultationService.ConfirmConsultationAsync(consultantId, id);
            return Ok(BaseResponse.OkMessageResponse("Confirmed consultation successfully"));
        }

        /// <summary>
        /// Update consultation result after the session is complete.
        /// </summary>
        /// <param name="id">The ID of the consultation.</param>
        /// <param name="request">The updated result of the consultation.</param>
        /// <returns>Success message after result is updated.</returns>
        [Authorize(Roles = "Consultant")]
        [HttpPut("{id}/result")]
        public async Task<IActionResult> UpdateResult(string id, [FromBody] UpdateConsultationResultRequest request)
        {
            var consultantId = await _userContextService.GetConsultantIdAsync();
            await _consultationService.UpdateConsultationResultAsync(consultantId, id, request.Result);

            return Ok(BaseResponse.OkMessageResponse("Updated result sucessfully for consultation"));
        }
    }
}
