namespace HostelFinder.Application.DTOs.MembershipService.Responses;

public class PostingMemberShipServiceDto
{
    public Guid Id { get; set; }
    public string TypeOfPost { get; set; }
    public int NumberOfPostsRemaining { get; set; }
    public int NumberOfPushTopRemaining { get; set; }
}