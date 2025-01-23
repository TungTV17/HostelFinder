using HostelFinder.Application.DTOs.AddressStory;
using HostelFinder.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Story.Requests
{
    public class UpdateStoryRequestDto
    {
        [MaxLength(512)]
        public string Title { get; set; }

        public decimal MonthlyRentCost { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public decimal Size { get; set; }

        public RoomType RoomType { get; set; }

        public DateTime DateAvailable { get; set; }

        public AddressStoryDto AddressStory { get; set; }

    }
}
