namespace HostelFinder.Application.DTOs.Users.Response
{
    public class LandlordResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
