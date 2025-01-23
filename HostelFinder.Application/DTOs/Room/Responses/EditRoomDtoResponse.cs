using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Room.Responses;

public class EditRoomDtoResponse
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
    public List<Guid> AmenityIds{ get; set; }
    public List<string> ImageRoom { get; set; }
}