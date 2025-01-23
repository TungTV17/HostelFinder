using HostelFinder.Application.DTOs.RoomDetails.Request;
using HostelFinder.Application.DTOs.ServiceCost.Request;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Room.Requests
{
    public class UpdateRoomRequestDto
    {
        public Guid HostelId { get; set; }
        public string RoomName { get; set; }
        public int? Floor { get; set; }
        public int MaxRenters { get; set; }
        public decimal Deposit { get; set; }
        public decimal MonthlyRentCost { get; set; }

        public decimal Size { get; set; }
        public RoomType RoomType { get; set; }
        public bool IsAvailable {  get; set; }
        public List<Guid> AmenityId{ get; set; }
    }
}
