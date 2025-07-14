using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.ReportModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUserContextService _userContextService;

        public ReportController(IUserContextService userContextService, IPaymentService paymentService)
        {
            _userContextService = userContextService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Exports transaction history as CSV or PDF.
        /// </summary>
        /// <param name="fromDate">Start date filter (optional).</param>
        /// <param name="toDate">End date filter (optional).</param>
        /// <param name="format">Export format: 'csv' or 'pdf' (default is 'csv').</param>
        /// <returns>Exported file.</returns>
        [HttpGet("transactions/export")]
        public async Task<IActionResult> ExportTransactions([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] string format = "csv")
        {
            var userId = _userContextService.GetUserId();
            var exportResult = await _paymentService.ExportTransactionsAsync(userId, fromDate, toDate, format.ToLower());

            return File(exportResult.FileBytes, exportResult.ContentType, exportResult.FileName);
        }

        /// <summary>
        /// Retrieves list of generated reports.
        /// </summary>
        /// <returns>List of report records.</returns>
        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _paymentService.GetReportsAsync();
            return Ok(BaseResponseModel<List<ReportResponseModel>>.OkDataResponse(reports, "Reports fetched successfully"));
        }

        [HttpGet("{reportId}/download")]
        public async Task<IActionResult> DownloadReport(string reportId)
        {
            var file = await _paymentService.DownloadReportFileAsync(reportId);
            return File(file.FileBytes, file.ContentType, file.FileName);
        }
    }

}
