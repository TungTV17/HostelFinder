namespace HostelFinder.Application.DTOs.MembershipService.Requests
{
    public class UpdateMembershipServiceReqDto
    {
        public string ServiceName { get; set; }
        public int MaxPushTopAllowed { get; set; }
        public int MaxPostsAllowed { get; set; }
    }
}
