using HostelFinder.Application.DTOs.Auth.Requests;
using HostelFinder.Application.DTOs.Auth.Responses;
using HostelFinder.Application.DTOs.Auths.Requests;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthAccountService _authAccountService;
        private readonly HostelFinderDbContext _context;

        public AuthController(IUserService userService, IAuthAccountService authAccountService, HostelFinderDbContext context)
        {
            _userService = userService;
            _authAccountService = authAccountService;
            _context = context;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                var response = await _userService.RegisterUserAsync(request);

                if (!response.Succeeded)
                {
                    return BadRequest(response); 
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            try
            {
                var response = await _authAccountService.LoginAsync(request);
                if (!response.Succeeded)
                {
                    return BadRequest(response); 
                }
                return Ok(response);  
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<AuthenticationResponse>
                {
                    Message = $"Error: {ex.Message}",
                    Succeeded = false
                };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var response = await _authAccountService.ChangePasswordAsync(request);
                if (!response.Succeeded)
                {
                    return BadRequest(response);  
                }
                return Ok(response);  
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<string>
                {
                    Message = $"Error: {ex.Message}",
                    Succeeded = false
                };

                return StatusCode(500, errorResponse); 
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var response = await _authAccountService.ForgotPasswordAsync(request);
                if (!response.Succeeded)
                {
                    return BadRequest(response); 
                }
                return Ok(response); 
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<string>
                {
                    Message = $"Error: {ex.Message}",
                    Succeeded = false
                };

                return StatusCode(500, errorResponse);  
            }
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var response = await _authAccountService.ResetPasswordAsync(request);
                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] LoginWithGoogleRequest request)
        {
            try
            {
                var response = await _authAccountService.LoginWithGoogleAsync(request);
                if (!response.Succeeded)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
       
    }
}