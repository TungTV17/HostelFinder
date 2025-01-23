using HostelFinder.Domain.Common;
using HostelFinder.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Domain.Entities
{
    public class Story : BaseEntity
    {
        public Guid? UserId { get; set; }
        [Required]
        [MaxLength(512)]
        public string Title { get; set; }
        public decimal MonthlyRentCost { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Description { get; set; }
        public decimal Size { get; set; }
        public RoomType RoomType { get; set; }
        public BookingStatus BookingStatus { get; set; } = 0;
        public DateTime DateAvailable { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual AddressStory AddressStory { get; set; }
        public virtual User User { get; set; }
    }
}
