using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMembershipController : ControllerBase
    {
        private readonly IUserMembershipService _userMembershipService;

        public UserMembershipController(IUserMembershipService userMembershipService)
        {
            _userMembershipService = userMembershipService;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetMembershipStatistics([FromQuery] string timeRange, [FromQuery] DateTime? customStartDate, [FromQuery] DateTime? customEndDate)
        {
            var result = await _userMembershipService.GetMembershipStatisticsAsync(timeRange, customStartDate, customEndDate);
            if (!result.Succeeded)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(result);
        }
    }
}
