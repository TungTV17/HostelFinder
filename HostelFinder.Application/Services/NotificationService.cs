
using AutoMapper;
using HostelFinder.Application.DTOs.Notification;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;

namespace HostelFinder.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<List<NotificationResponseDto>> GetMessagesByUserIdAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);

            if (notifications == null || notifications.Count == 0)
            {
                return null;
            }

            var notificationDtos = _mapper.Map<List<NotificationResponseDto>>(notifications);

            for (int i = 0; i < notificationDtos.Count; i++)
            {
                notificationDtos[i].TimeAgo = GetTimeAgo(notifications[i].CreatedOn);
            }

            return notificationDtos;
        }

        private string GetTimeAgo(DateTimeOffset createdOn)
        {
            var timeDifference = DateTimeOffset.Now - createdOn.ToUniversalTime();

            if (timeDifference.Days > 365)
            {
                return $"{timeDifference.Days / 365} năm trước";
            }
            if (timeDifference.Days > 30)
            {
                return $"{timeDifference.Days / 30} tháng trước";
            }
            if (timeDifference.Days > 0)
            {
                return $"{timeDifference.Days} ngày trước";
            }
            if (timeDifference.Hours > 0)
            {
                return $"{timeDifference.Hours} giờ trước";
            }
            if (timeDifference.Minutes > 0)
            {
                return $"{timeDifference.Minutes} phút trước";
            }
            return "ngay bây giờ";
        }
    }
}
