using HostelFinder.Application.DTOs.Hostel.Requests;
using HostelFinder.Application.DTOs.Hostel.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/hostels")]
    [ApiController]
    public class HostelController : ControllerBase
    {
        private readonly IHostelService _hostelService;
        private readonly IS3Service _s3Service;

        public HostelController(IHostelService hostelService, IS3Service s3Service)
        {
            _hostelService = hostelService;
            _s3Service = s3Service;
        }

        [HttpGet("{hostelId}")]
        public async Task<IActionResult> GetHostelById(Guid hostelId)
        {
            try
            {
                var result = await _hostelService.GetHostelByIdAsync(hostelId);
                if (!result.Succeeded || result.Data == null)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<HostelResponseDto>
                {
                    Succeeded = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("GetHostelsByLandlordId/{landlordId}")]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> GetHostelsByLandlordId(Guid landlordId, string? searchPhrase, int? pageNumber, int? pageSize,string? sortBy, SortDirection? sortDirection)
        {
            try
            {
                var hostels = await _hostelService.GetHostelsByUserIdAsync(landlordId,searchPhrase, pageNumber, pageSize, sortBy, sortDirection);
                if (hostels.Succeeded && hostels.Data != null)
                {
                    return Ok(hostels);
                }

                return NotFound(hostels.Errors ?? new List<string> { "No hostels found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> AddHostel([FromForm] AddHostelRequestDto hostelDto, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imageUrl = null;

            // Kiểm tra nếu có hình ảnh
            if (image != null)
            {
                try
                {
                    // Tải ảnh lên AWS S3 và lấy URL
                    imageUrl = await _s3Service.UploadFileAsync(image);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Không thể tải ảnh lên: {ex.Message}");
                }
            }

            try
            {
                // Gọi phương thức AddHostelAsync với chỉ một hình ảnh
                var result = await _hostelService.AddHostelAsync(hostelDto, imageUrl);
                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpPut("{hostelId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> UpdateHostel(Guid hostelId, [FromForm] UpdateHostelRequestDto request, IFormFile? image)
        {
            try
            {
                var result = await _hostelService.UpdateHostelAsync(hostelId, request, image);
        
                if (result.Succeeded)
                {
                    return Ok(result);  
                }

                // Trả về 400 nếu có lỗi về dữ liệu yêu cầu
                return BadRequest(new Response<HostelResponseDto>
                {
                    Succeeded = false,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 khi có ngoại lệ
                return StatusCode(500, new Response<HostelResponseDto>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }


        [HttpDelete("DeleteHostel/{id}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> DeleteHostel(Guid id)
        {
            try
            {
                var result = await _hostelService.DeleteHostelAsync(id);
                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPost("get-all")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetAll([FromBody] GetAllHostelQuery request)
        {
            try
            {
                var response = await _hostelService.GetAllHostelAsync(request);
                if (!response.Succeeded || response.Data == null || !response.Data.Any())
                {
                    return NotFound(new PagedResponse<List<ListHostelResponseDto>>
                    {
                        Succeeded = false,
                        Message = "No hostels found."
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetDashboardForLandlord")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetDashboard(Guid landlordId)
        {
            try
            {
                var dashboardData = await _hostelService.GetDashboardDataAsync(landlordId);

                if (dashboardData == null)
                {
                    return NotFound(new { message = "Dashboard data not found for the given landlordId." });
                }

                return Ok(dashboardData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}