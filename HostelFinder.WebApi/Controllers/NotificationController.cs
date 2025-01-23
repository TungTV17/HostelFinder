using HostelFinder.Application.DTOs.Notification;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("messages/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetMessagesByUserId(Guid userId)
        {
            try
            {
                var notificationDtos = await _notificationService.GetMessagesByUserIdAsync(userId);

                if (notificationDtos == null || notificationDtos.Count == 0)
                {
                    return NotFound(new { Message = "No notifications found for this user." });
                }

                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }

        }
    }
}
