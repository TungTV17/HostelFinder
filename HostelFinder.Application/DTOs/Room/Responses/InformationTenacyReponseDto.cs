namespace HostelFinder.Application.DTOs.Room.Responses
{
    public class InformationTenacyReponseDto
    {
        public Guid TenantId { get; set; }
        public required string FullName { get; set; }

        public string? AvatarUrl { get; set; }

        public required string Email { get; set; }
        public required string Phone { get; set; }

        public string? Province { get; set; }

        public string? IdentityCardNumber { get; set; }

        public string? Description { get; set; }

        public DateTime MoveInDate { get; set; }
    }
}
