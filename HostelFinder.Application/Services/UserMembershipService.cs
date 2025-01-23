using AutoMapper;
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Services
{

    public class UserMembershipService : IUserMembershipService
    {
        private readonly IUserMembershipRepository _userMembershipRepository;
        private readonly IMapper _mapper;

        public UserMembershipService(IUserMembershipRepository userMembershipRepository, IMapper mapper)
        {
            _userMembershipRepository = userMembershipRepository;
            _mapper = mapper;
        }

        public async Task<Response<UserMembershipStatistics>> GetMembershipStatisticsAsync(string timeRange, DateTime? customStartDate = null, DateTime? customEndDate = null)
        {
            DateTime startDate;
            DateTime endDate;

            switch (timeRange.ToLower())
            {
                case "today":
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1); // Kết thúc vào cuối ngày hôm nay
                    break;
                case "yesterday":
                    startDate = DateTime.Today.AddDays(-1);
                    endDate = DateTime.Today.AddTicks(-1); // Kết thúc vào cuối ngày hôm qua
                    break;
                case "thisweek":
                    startDate = GetStartOfWeek(DateTime.Now);
                    endDate = GetEndOfWeek(DateTime.Now);
                    break;
                case "thismonth":
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    break;
                case "thisyear":
                    startDate = new DateTime(DateTime.Now.Year, 1, 1);
                    endDate = new DateTime(DateTime.Now.Year, 12, 31);
                    break;
                case "custom":
                    if (!customStartDate.HasValue)
                    {
                        return new Response<UserMembershipStatistics> { Message = "Cần phải cung cấp ngày bắt đầu cho tùy chọn 'custom'", Succeeded = false };
                    }
                    startDate = customStartDate.Value;
                    endDate = customEndDate ?? DateTime.Now;
                    break;
                default:
                    return new Response<UserMembershipStatistics> { Message = "Thời gian không hợp lệ", Succeeded = false };
            }

            // Lấy dữ liệu UserMembership từ repository
            var memberships = await _userMembershipRepository.GetUserMembershipsAsync(startDate, endDate);

            // Tính toán thống kê
            var statistics = new UserMembershipStatistics
            {
                TotalMemberships = memberships.Count(),
                TotalPaidMemberships = memberships.Count(um => um.IsPaid),
                TotalPostsUsed = memberships.Sum(um => um.PostsUsed),
                TotalPushTopUsed = memberships.Sum(um => um.PushTopUsed),
                TotalPrice = memberships.Sum(um => um.Membership.Price * (um.IsPaid ? 1 : 0)),  // Giá chỉ tính cho các membership đã được thanh toán
                MembershipDetails = memberships.GroupBy(um => um.MembershipId)
                                              .Select(group => new MembershipDetail
                                              {
                                                  MembershipName = group.First().Membership.Name,
                                                  TotalPrice = group.Sum(um => um.Membership.Price * (um.IsPaid ? 1 : 0)),
                                                  TotalUsers = group.Count(),
                                              }).ToList()
            };

            return new Response<UserMembershipStatistics> { Message = "Thống kê thành công", Succeeded = true, Data = statistics };
        }

        // Phương thức lấy ngày đầu tuần
        private DateTime GetStartOfWeek(DateTime date)
        {
            var diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return date.AddDays(-diff).Date;
        }

        // Phương thức lấy ngày cuối tuần
        private DateTime GetEndOfWeek(DateTime date)
        {
            var diff = DayOfWeek.Sunday - date.DayOfWeek;
            if (diff < 0) 
                diff += 7; 
            return date.AddDays(diff).Date.AddDays(1).AddTicks(-1);
        }
    }
}
