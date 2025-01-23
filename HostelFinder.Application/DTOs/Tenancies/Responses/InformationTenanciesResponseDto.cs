namespace HostelFinder.Application.DTOs.Tenancies.Responses
{
    public class InformationTenanciesResponseDto
    {
        public Guid TenancyId { get; set; }
        public Guid RoomId {  get; set; }
        public Guid HostelId { get; set; }
        public string RoomName {  get; set; }
        public required string FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public string? MoveInDate { get; set; }
        public string? MoveOutDate { get; set; }
        public string Status { get; set; }
    }
}
