using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.RoomDetails.Request;

public class AddRoomDetailRequestDto
{
    public int BedRooms { get; set; } 
    public int BathRooms { get; set; }
    public int Kitchen { get; set; }
    public decimal Size { get; set; }
}