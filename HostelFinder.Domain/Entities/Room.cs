using HostelFinder.Domain.Enums;
using RoomFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class Room : BaseEntity
    {
        public Guid HostelId { get; set; }
        public string? RoomName {  get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public int? Floor { get; set; } 
        public int MaxRenters { get; set; }
        public decimal Size { get; set; }
        public bool IsAvailable {  get; set; }
        public decimal MonthlyRentCost { get; set; }
        public decimal Deposit {  get; set; }
        public RoomType RoomType { get; set; }

        //Navigation
        public virtual Hostel Hostel { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<RoomAmenities> RoomAmenities { get; set; } 
        public virtual RoomDetails RoomDetails { get; set; }

        public virtual ICollection<Image>? Images { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } 

        public virtual ICollection<MeterReading> MeterReadings { get; set; }

        public virtual ICollection<RoomTenancy> RoomTenancies { get; set; } = new List<RoomTenancy>();

        public virtual ICollection<MaintenanceRecord>? MaintenanceRecords { get; set; }

    }
}