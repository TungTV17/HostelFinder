using HostelFinder.Application.DTOs.MaintenanceRecord.Request;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers;
[ApiController]
[Route("api/maintenance-record")]
[Authorize("Landlord")]
public class MaintenanceRecordController : ControllerBase
{
    private readonly IMaintenanceRecordService _maintenanceRecordService;
    
    public MaintenanceRecordController(IMaintenanceRecordService maintenanceRecordService)
    {
        _maintenanceRecordService = maintenanceRecordService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMaintenanceRecordAsync([FromBody] CreateMaintenanceRecordRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request is required.");
        }
        
        try
        {
            var response = await _maintenanceRecordService.AddMaintenanceRecordAsync(request);
            if (!response.Succeeded)
            {   
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllMaintenanceRecordAsync([FromQuery] GetAllMaintenanceRecordQuery request)
    {
        try
        {
            var response = await _maintenanceRecordService.GetAllMaintenanceRecordAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] EditMaintenanceRecordDto dto)
    {
        try
        {
            var response = await _maintenanceRecordService.EditMaintenanceRecordAsync(id, dto);

            if (!response.Succeeded)
            {
                return NotFound(response.Errors);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var response = await _maintenanceRecordService.DeleteMaintenanceRecordAsync(id);

            if (!response.Succeeded)
            {
                return NotFound(response.Errors);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _maintenanceRecordService.GetMaintenanceRecordByIdAsync(id);

            if (!response.Succeeded)
            {
                return NotFound(new { message = response.Message });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
        }
    }

}