using HostelFinder.Application.DTOs.Membership.Requests;
using HostelFinder.Application.DTOs.Payment.Requests;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IWalletService _walletService;

        public MembershipController(IMembershipService membershipService, IWalletService walletService)
        {
            _membershipService = membershipService;
            _walletService = walletService;
        }

        [HttpGet("GetListMembership")]
        public async Task<IActionResult> GetListMembership()
        {
            var response = await _membershipService.GetAllMembershipWithMembershipService();
            if (!response.Succeeded || response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("AddMembership")]
        public async Task<IActionResult> AddMembership([FromBody] AddMembershipRequestDto membershipDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _membershipService.AddMembershipAsync(membershipDto);
                if (response.Data != null)
                {
                    return Ok(response);
                }

                return BadRequest(response.Errors);
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPut("EditMembership/{id}")]
        public async Task<IActionResult> EditMembership(Guid id, [FromBody] UpdateMembershipRequestDto membershipDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _membershipService.EditMembershipAsync(id, membershipDto);

                if (!response.Succeeded)
                {
                    return NotFound(response); 
                }

                return Ok(response); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }

        [HttpDelete("DeleteMembership/{id}")]
        public async Task<IActionResult> DeleteMembership(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new Response<string>
                    {
                        Succeeded = false,
                        Message = "Invalid membership ID."
                    });
                }

                var response = await _membershipService.DeleteMembershipAsync(id);
                if (!response.Succeeded)
                {
                    return NotFound(new Response<string>
                    {
                        Succeeded = false,
                        Message = response.Message
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

        [HttpGet("MembershipServices/{userId}")]
        public async Task<IActionResult> GetMembershipServicesForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid user ID."
                });
            }

            try
            {
                var response = await _membershipService.GetMembershipServicesForUserAsync(userId);
                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return NotFound(response);
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

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromForm] WalletDepositRequestDto depositRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>("Invalid deposit request."));
            }

            var depositResponse = await _walletService.DepositAsync(depositRequest);
            if (!depositResponse.Succeeded)
            {
                return BadRequest(depositResponse);
            }

            return Ok(depositResponse);
        }

        [HttpGet("CheckTransactionStatus/{orderCode}")]
        public async Task<IActionResult> CheckTransactionStatus(long orderCode)
        {
            var response = await _walletService.CheckTransactionStatusAsync(orderCode);
            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        
    }
}