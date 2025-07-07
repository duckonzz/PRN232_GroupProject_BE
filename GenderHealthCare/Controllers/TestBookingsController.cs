﻿using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Models;
using GenderHealthCare.ModelViews.TestBookingModel;
using Microsoft.AspNetCore.Mvc;

namespace GenderHealthCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestBookingsController : ControllerBase
    {
        private readonly ITestBookingService _service;
        public TestBookingsController(ITestBookingService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetAllAsync(page, size);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string? status,
            [FromQuery] string? customerId,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _service.SearchAsync(status, customerId, page, size);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}", Name = "GetTestBookingById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTestBookingDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);

            return CreatedAtRoute("GetTestBookingById",
                                  new { id = result.Data },
                                  result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateTestBookingDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
