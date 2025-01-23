using HostelFinder.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Room.Responses
{
    public class RoomInfoDetailResponseDto
    {
        public Guid RoomId { get; set; }
        public string? RoomName { get; set; }
        [MaxLength(500)]
        public int? Floor { get; set; }
        public int MaxRenters { get; set; }
        public int? NumberOfCustomer { get; set; }
        public decimal Size { get; set; }
        public bool IsAvailable { get; set; }
        public decimal MonthlyRentCost { get; set; }
        public RoomType RoomType { get; set; }
    }
}
