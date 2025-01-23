namespace HostelFinder.Application.DTOs.Auth.Responses
{
    public class AuthenticationResponse
    {
        public string UserName { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }
    }
}
