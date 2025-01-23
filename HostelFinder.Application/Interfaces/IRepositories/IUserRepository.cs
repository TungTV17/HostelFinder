using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IUserRepository : IBaseGenericRepository<User>
    {
        Task<User?> FindByUserNameAsync(string userName);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckUserNameExistAsync(string userName);
        Task<bool> CheckEmailExistAsync(string email);
        Task<bool> CheckPhoneNumberAsync(string phoneNumber);
        Task<UserRole> GetRoleAsync(Guid userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetLandlordByHostelIdAsync(Guid hostelId);
        Task<Hostel> GetHostelByPostIdAsync(Guid postId);
        Task<User> GetUserByHostelIdAsync(Guid hostelId);
        Task<IEnumerable<User>> FilterUsersByActiveStatusAsync(bool isActive);
    }
}
