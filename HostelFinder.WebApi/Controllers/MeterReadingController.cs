using HostelFinder.Application.DTOs.MeterReading.Request;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/meterReadings")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;
        public MeterReadingController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        [HttpPost]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> AddMeterReading([FromBody] CreateMeterReadingDto createMeterReadingDto)
        {
            try
            {
                var response = await _meterReadingService.AddMeterReadingAsync(createMeterReadingDto.roomId, createMeterReadingDto.serviceId, createMeterReadingDto.previousReading, createMeterReadingDto.currentReading, createMeterReadingDto.billingMonth, createMeterReadingDto.billingYear);
                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("list")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> AddMeterReadingList([FromBody] List<CreateMeterReadingDto> createMeterReadingDtos)
        {
            try
            {
                var response = await _meterReadingService.AddMeterReadingListAsync(createMeterReadingDtos);
                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{hostelId}/{roomId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetServiceCostReading(Guid hostelId, Guid roomId, int? billingMonth, int? billingYear)
        {
            try
            {
                var serviceCostReading = await _meterReadingService.GetServiceCostReadingAsync(hostelId, roomId, billingMonth, billingYear);
                return Ok(serviceCostReading);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedMeterReadings(
                [FromForm] Guid hostelId,
                [FromQuery] int pageIndex = 1,
                [FromQuery] int pageSize = 10,
                [FromForm] string? roomName = null,
                [FromForm] int? month = null,
                [FromForm] int? year = null)
        {
            try
            {
                var response = await _meterReadingService.GetPagedMeterReadingsAsync(pageIndex, pageSize, hostelId, roomName, month, year);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromForm] EditMeterReadingDto dto)
        {
            try
            {
                var response = await _meterReadingService.EditMeterReadingAsync(id, dto);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the meter reading.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var response = await _meterReadingService.DeleteMeterReadingAsync(id);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the meter reading.", error = ex.Message });
            }
        }

    }
}
