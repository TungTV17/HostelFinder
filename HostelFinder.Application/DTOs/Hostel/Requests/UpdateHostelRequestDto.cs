using HostelFinder.Application.DTOs.Address;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Hostel.Requests
{
    public class UpdateHostelRequestDto
    {
        [Required]
        public string HostelName { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public AddressDto Address { get; set; }
        public float? Size { get; set; }
        public int NumberOfRooms { get; set; }
        public string? Coordinates { get; set; }
        public List<Guid?>? ServiceId { get; set; }
    }
}
