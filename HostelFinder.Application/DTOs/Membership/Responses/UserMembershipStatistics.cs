
namespace HostelFinder.Application.DTOs.Membership.Responses
{
    public class UserMembershipStatistics
    {
        public int TotalMemberships { get; set; }
        public int TotalPaidMemberships { get; set; }
        public int TotalPostsUsed { get; set; }
        public int TotalPushTopUsed { get; set; }
        public decimal TotalPrice { get; set; }
        public List<MembershipDetail> MembershipDetails { get; set; }
    }
}
