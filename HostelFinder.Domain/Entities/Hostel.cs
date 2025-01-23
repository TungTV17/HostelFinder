using RoomFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class Hostel : BaseEntity
    {
        [ForeignKey("User")]
        public Guid? LandlordId { get; set; }
        [Required]
        [MaxLength(50)]
        public string HostelName { get; set; } 
        [MaxLength(1000)]
        public string? Description { get; set; } 
        [Required]
        public float? Size { get; set; }
        [Required]
        public int NumberOfRooms { get; set; }
        public string? Coordinates { get; set; }
        [JsonIgnore]
        public virtual ICollection<HostelServices> HostelServices { get; set; } 
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual User Landlord { get; set; } 
        public virtual Address Address { get; set; } 
        public virtual ICollection<ServiceCost> ServiceCosts { get; set; } = new List<ServiceCost>();
        public virtual ICollection<MaintenanceRecord>? MaintenanceRecords { get; set; }
    }
}