using HostelFinder.Application.DTOs.Address;
using HostelFinder.Application.DTOs.Service.Response;

namespace HostelFinder.Application.DTOs.Hostel.Responses
{
    public class HostelResponseDto
    {
        public Guid Id { get; set; }
        public string HostelName { get; set; }
        public string? Description { get; set; }
        public AddressDto Address { get; set; }
        public int Size { get; set; }
        public int NumberOfRooms { get; set; }
        public string ImageUrl { get; set; }
        public string Coordinates { get; set; }
        public List<HostelServiceResponseDto> Services { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
