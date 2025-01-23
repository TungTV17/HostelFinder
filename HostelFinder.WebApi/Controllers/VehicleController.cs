using HostelFinder.Application.DTOs.Vehicle.Request;
using HostelFinder.Application.DTOs.Vehicle.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("tenant/{tenantId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetVehicleByTenant(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
            {
                var errorResponse = new Response<IEnumerable<VehicleResponseDto>>
                {
                    Succeeded = false,
                    Message = "Invalid tenantId"
                };
                return BadRequest(errorResponse); 
            }

            try
            {
                var response = await _vehicleService.GetVehicleByTenantAsync(tenantId);

                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return NotFound(new Response<IEnumerable<VehicleResponseDto>>
                {
                    Succeeded = false,
                    Message = response.Message ?? "Không tìm thấy xe cho người thuê trọ"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<IEnumerable<VehicleResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Có lỗi xảy ra: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Thêm xe mới
        [HttpPost]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> AddVehicle([FromForm] AddVehicleDto request)
        {
            if (request == null)
            {
                return BadRequest(new Response<VehicleResponseDto>
                {
                    Succeeded = false,
                    Message = "Request body cannot be empty."
                });
            }

            try
            {
                var response = await _vehicleService.AddVehicleAsync(request);
                if (response.Succeeded)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<VehicleResponseDto>
                {
                    Succeeded = false,
                    Errors = new List<string> { $"Có lỗi xảy ra: {ex.Message}" }
                });
            }
        }

        // Lấy thông tin xe theo ID
        [HttpGet("{vehicleId}")]
        public async Task<IActionResult> GetVehicleById(Guid vehicleId)
        {
            try
            {
                var response = await _vehicleService.GetVehicleByIdAsync(vehicleId);
                if (response.Succeeded)
                {
                    return Ok(response);  
                }
                else
                {
                    return NotFound(response); 
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<VehicleResponseDto>
                {
                    Succeeded = false,
                    Errors = new List<string> { $"Có lỗi xảy ra: {ex.Message}" }
                });
            }
        }

        // Lấy danh sách tất cả xe
        [HttpGet]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var response = await _vehicleService.GetAllVehiclesAsync();
                if (response.Succeeded)
                    return Ok(response);
                else
                    return NotFound(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<IEnumerable<VehicleResponseDto>>
                {
                    Succeeded = false,
                    Errors = new List<string> { $"Có lỗi xảy ra: {ex.Message}" }
                });
            }
        }

        // Cập nhật xe
        [HttpPut("{vehicleId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> UpdateVehicle(Guid vehicleId, [FromForm] AddVehicleDto request)
        {
            if (request == null)
            {
                return BadRequest(new Response<VehicleResponseDto>
                {
                    Succeeded = false,
                    Message = "Request body cannot be empty."
                });
            }

            try
            {
                var response = await _vehicleService.UpdateVehicleAsync(vehicleId, request);

                if (response.Succeeded)
                    return Ok(response);
                else
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<VehicleResponseDto>
                {
                    Succeeded = false,
                    Errors = new List<string> { $"Có lỗi xảy ra: {ex.Message}" }
                });
            }
        }

        // Xóa xe
        [HttpDelete("{vehicleId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> DeleteVehicle(Guid vehicleId)
        {
            try
            {
                var response = await _vehicleService.DeleteVehicleAsync(vehicleId);

                if (response.Succeeded)
                    return Ok(response);
                else
                    return NotFound(response); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Succeeded = false,
                    Errors = new List<string> { $"Có lỗi xảy ra: {ex.Message}" }
                });
            }
        }
    }
}
