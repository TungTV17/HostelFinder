using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HostelFinder.Domain.Common;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Domain.Entities;

public class MaintenanceRecord : BaseEntity
{
    [ForeignKey(nameof(Hostel))]
    public Guid HostelId { get; set; }

    [ForeignKey(nameof(Room))]
    public Guid? RoomId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime MaintenanceDate { get; set; }

    public decimal Cost { get; set; }

    
    public MaintenanceType MaintenanceType { get; set; }

    // Navigation properties
    public virtual Hostel Hostel { get; set; }
    public virtual Room? Room { get; set; }
}