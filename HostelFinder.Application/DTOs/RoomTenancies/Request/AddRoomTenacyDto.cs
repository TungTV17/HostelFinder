namespace HostelFinder.Application.DTOs.RoomTenancies.Request
{
    public class AddRoomTenacyDto
    {
        public Guid RoomId { get; set; }
        public Guid TenantId { get; set; }
    }
}
