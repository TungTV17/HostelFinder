namespace HostelFinder.Application.DTOs.Post.Requests;

public class UpdatePostRequestDto
{
    public Guid HostelId { get; set; }
    public Guid RoomId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Status { get; set; }
    public DateTime DateAvailable { get; set; }
}