namespace HostelFinder.Application.DTOs.MembershipService.Requests
{
    public class AddMembershipServiceReqDto
    {
        public string ServiceName { get; set; }
        public int MaxPushTopAllowed {  get; set; }
        public int MaxPostsAllowed { get; set; }
    }
}
