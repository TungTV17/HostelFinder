namespace HostelFinder.Application.DTOs.Post.Responses;

public class PostResponseDto
{
   public Guid Id { get; set; }
    public Guid HostelId { get; set; }
    public Guid RoomId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> ImageUrls { get; set; }
    public bool Status { get; set; }
    public DateTime DateAvailable { get; set; }
    public Guid MembershipServiceId { get; set; }
}