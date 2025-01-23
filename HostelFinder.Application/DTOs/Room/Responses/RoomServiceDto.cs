namespace HostelFinder.Application.DTOs.Room.Responses
{
    public class RoomServiceDto
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public bool IsBillable { get; set; }
        public bool IsUsageBased { get; set; }

        public decimal UnitCost { get; set; }


    }
}
