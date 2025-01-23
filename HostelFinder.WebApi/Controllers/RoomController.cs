using HostelFinder.Application.DTOs.RentalContract.Request;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.DTOs.RoomTenancies.Request;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ITenantService _tenantService;
        private readonly IRoomTenancyService _roomTenancyService;
        public RoomController(IRoomService roomService,
            ITenantService tenantService,
            IRoomTenancyService roomTenancyService)
        {
            _roomService = roomService;
            _tenantService = tenantService;
            _roomTenancyService = roomTenancyService;
        }

        [HttpGet]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                var response = await _roomService.GetAllAsync();

                if (!response.Succeeded)
                {
                    return BadRequest(new Response<List<RoomResponseDto>>
                    {
                        Succeeded = false,
                        Message = response.Message ?? "Failed to retrieve rooms."
                    });
                }

                if (response.Data == null || !response.Data.Any())
                {
                    return Ok(new Response<List<RoomResponseDto>>
                    {
                        Succeeded = true,
                        Data = new List<RoomResponseDto>(),
                        Message = "No rooms found"
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(Guid id)
        {
            try
            {
                var response = await _roomService.GetByIdAsync(id);
                if (!response.Succeeded)
                    return NotFound(response.Message);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> CreateRoom([FromForm] AddRoomRequestDto roomDto, [FromForm] List<IFormFile> roomImages)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //map to Domain Room 
                var response = await _roomService.CreateRoomAsync(roomDto, roomImages);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> UpdateRoom(Guid id, [FromForm] UpdateRoomRequestDto roomDto, [FromForm] List<IFormFile> roomImages)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _roomService.UpdateAsync(id, roomDto, roomImages);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new Response<string>
                    {
                        Succeeded = false,
                        Message = "Invalid room ID"
                    });
                }

                var response = await _roomService.DeleteAsync(id);

                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("hostels/{hostelId}")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetRoomsByHostelId(Guid hostelId, int? floor)
        {
            try
            {
                var response = await _roomService.GetRoomsByHostelIdAsync(hostelId, floor);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("info-retancy")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetInfoRentacyRoom(Guid roomId)
        {
            var response = await _tenantService.GetInformationTenacyAsync(roomId);
            return Ok(response);
        }

        [HttpGet]
        [Route("info-detail")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetInfoDetailRoom(Guid roomId)
        {
            var response = await _roomService.GetInformationDetailRoom(roomId);
            if (!response.Succeeded)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("AddTenantToRoom")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> AddTenantToRoom([FromForm] AddRoomTenacyDto request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu yêu cầu không hợp lệ.");
            }

            try
            {
                var response = await _roomTenancyService.AddTenantToRoomAsync(request);

                if (response.Succeeded)
                {
                    return Ok(response);  
                }
                else
                {
                    return BadRequest(response.Message); 
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        [HttpPost("AddRoommate")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> AddRoommate([FromForm] AddRoommateDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Dữ liệu yêu cầu không hợp lệ.");
                }

                var response = await _tenantService.AddRoommateAsync(request);

                if (response.Succeeded)
                {
                    return Ok(response.Message); // Trả về thông báo thành công
                }

                return BadRequest(response.Message); // Trả về thông báo lỗi nếu không thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
        
        [HttpGet("get-room-with-amentities-and-image")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> GetRoomWithAmentitiesAndImage(Guid roomId)
        {
            try
            {
                var response = await _roomService.GetRoomWithAmentitesAndImageAsync(roomId);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }

        }

        [HttpGet("check-delete-room")]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> CheckDeleteRoom(Guid roomId)
        {
            try
            {
                var response = await _roomService.CheckDeleteRoom(roomId);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
        
        [HttpGet("select-room/{hostelId}")]
        [Authorize(Roles = "Landlord")]
        
        public async Task<IActionResult> GetRoomsByHostelId(Guid hostelId)
        {
            try
            {
                var response = await _roomService.GetSelectRoomByHostelAsync(hostelId);
                if (!response.Succeeded)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
    }
}
