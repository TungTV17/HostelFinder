
namespace HostelFinder.Application.DTOs.Story.Requests
{
    public class StoryFilterDto
    {
        public decimal? MinRentCost { get; set; }
        public decimal? MaxRentCost { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public decimal? MinSize { get; set; }
        public decimal? MaxSize { get; set; }
    }
}
