using HostelFinder.Application.DTOs.Tenancies.Requests;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("GetAllTenantsByHostel/{hostelId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetAllTenantsByHostelAsync(
            [FromRoute] Guid hostelId,
            [FromQuery] string? roomName,
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result =
                await _tenantService.GetAllTenantsByHostelAsync(hostelId, roomName, pageNumber, pageSize, status);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpPost("moveout")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> MoveOut(Guid tenantId, Guid roomId)
        {
            if (tenantId == Guid.Empty || roomId == Guid.Empty)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Thông tin tenantId hoặc roomId không hợp lệ."
                });
            }

            var result = await _tenantService.MoveOutAsync(tenantId, roomId);

            if (!result.Succeeded)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = result.Message
                });
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> UpdateTenantAsync([FromForm] UpdateTenantDto request)
        {
            try
            {
                var result = await _tenantService.UpdateTenantAsync(request);
                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{tenantId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetTenantByIdAsync(Guid tenantId)
        {
            try
            {
                var result = await _tenantService.GetTenantByIdAsync(tenantId);
                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
