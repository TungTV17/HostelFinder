namespace HostelFinder.Application.DTOs.Users.Response
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public decimal WalletBalance { get; set; }
        public string QRCode { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
    }
}
