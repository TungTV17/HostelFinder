
using HostelFinder.Application.DTOs.Notification;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetMessagesByUserIdAsync(Guid userId);
    }
}
