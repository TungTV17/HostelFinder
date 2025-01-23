namespace HostelFinder.Application.DTOs.Users.Requests
{
    public class UpdateUserRequestDto
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}