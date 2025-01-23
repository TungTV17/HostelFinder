
using HostelFinder.Application.DTOs.AddressStory;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Story.Responses
{
    public class ListStoryResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public decimal MonthlyRentCost { get; set; }
        public string Description { get; set; }
        public decimal Size { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public RoomType RoomType { get; set; }
        public DateTime DateAvailable { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public AddressStoryDto AddressStory { get; set; }
        public string Images { get; set; }
    }
}
