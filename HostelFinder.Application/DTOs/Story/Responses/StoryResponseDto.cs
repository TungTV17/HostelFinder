
using HostelFinder.Application.DTOs.AddressStory;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Story.Responses
{
    public class StoryResponseDto
    {
        public string Title { get; set; }
        public decimal MonthlyRentCost { get; set; }
        public string Description { get; set; }
        public decimal Size { get; set; }
        public RoomType RoomType { get; set; }
        public DateTime DateAvailable { get; set; }
        public AddressStoryDto AddressStory { get; set; }
        public List<string> Images { get; set; }
    }
}
