using HostelFinder.Application.DTOs.Image.Requests;
using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.DTOs.Users.Response;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/GetListUser
        [HttpGet("GetListUser")]
        public async Task<IActionResult> GetListUser()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                if (!users.Succeeded)
                {
                    return NotFound(new Response<List<UserDto>>
                    {
                        Succeeded = false,
                        Message = users.Errors?.FirstOrDefault() ?? "No users found.",
                        Data = null
                    });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<List<UserDto>>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        // PUT: api/User/UpdateUser/{userId}
        [HttpPut("UpdateUser/{userId}")]
        [Authorize(Roles = "User, Landlord")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromForm] UpdateUserRequestDto request, [FromForm] UploadImageRequestDto? image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid model state."
                });
            }

            try
            {
                var result = await _userService.UpdateUserAsync(userId, request, image);
                if (!result.Succeeded)
                {
                    return NotFound(new Response<string>
                    {
                        Succeeded = false,
                        Message = result.Message
                    });
                }
                return Ok(result);
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

        // PUT: api/User/UnActiveUser/{userId}
        [HttpPut("UnActiveUser/{userId}")]
        public async Task<IActionResult> UnActiveUser(Guid userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Response<bool>
                    {
                        Succeeded = false,
                        Message = "Invalid model state."
                    });
                }

                var result = await _userService.UnActiveUserAsync(userId);
                if (!result.Succeeded)
                {
                    return BadRequest(new Response<bool>
                    {
                        Succeeded = false,
                        Message = result.Message,
                        Data = false
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = false
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new Response<UserProfileResponse>
                    {
                        Succeeded = false,
                        Message = "Invalid user ID."
                    });
                }

                var response = await _userService.GetUserByIdAsync(id);

                if (!response.Succeeded || response.Data == null)
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<UserProfileResponse>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("GetUserByHostelId/{hostelId}")]
        public async Task<IActionResult> GetUserByHostelId(Guid hostelId)
        {
            try
            {
                if (hostelId == Guid.Empty)
                {
                    return BadRequest(new Response<UserProfileResponse>
                    {
                        Succeeded = false,
                        Message = "Invalid hostel ID."
                    });
                }

                var response = await _userService.GetUserByHostelIdAsync(hostelId);

                if (!response.Succeeded || response.Data == null)
                {
                    return NotFound(new Response<UserProfileResponse>
                    {
                        Succeeded = false,
                        Message = "User not found for the given hostel ID."
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<UserProfileResponse>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPost("BuyMembership")]
        public async Task<IActionResult> BuyMembership(Guid userId, Guid membershipId)
        {
            // Gọi dịch vụ để xử lý nghiệp vụ mua/gia hạn membership
            var response = await _userService.ManageUserMembershipAsync(userId, membershipId);

            if (!response.Succeeded)
            {
                return BadRequest(new { response.Message });
            }

            return Ok(response);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterUsersByActiveStatus([FromForm] bool isActive)
        {
            try
            {
                var users = await _userService.FilterUsersByActiveStatusAsync(isActive);

                if (users == null || !users.Any())
                {
                    return NotFound(new { message = "No users found with the specified active status." });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
        
        //Upload QR code for user
        [HttpPost("UploadQRCode")]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> UploadQRCode(Guid landlordId,[FromForm] UploadQRCodeRequestDto request)
        {
            try
            {
                var response = await _userService.UploadQRCodeAsync(landlordId,request);
                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}