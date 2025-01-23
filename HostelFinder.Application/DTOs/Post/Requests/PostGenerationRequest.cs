namespace HostelFinder.Application.DTOs.Post.Requests;

public class PostGenerationRequest
{
    public Guid HostelId { get; set; }
    public Guid RoomId { get; set; }
}