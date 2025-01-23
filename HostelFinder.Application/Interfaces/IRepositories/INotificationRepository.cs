
using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface INotificationRepository : IBaseGenericRepository<Notification>
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId);
    }
}
