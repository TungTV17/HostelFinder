
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IUserMembershipService
    {
        Task<Response<UserMembershipStatistics>> GetMembershipStatisticsAsync(string timeRange, DateTime? customStartDate = null, DateTime? customEndDate = null);
    }
}
