using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.MaintenanceRecord.Request;

public class CreateMaintenanceRecordRequest
{
    public Guid HostelId { get; set; }

    public Guid? RoomId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime MaintenanceDate { get; set; }

    public decimal Cost { get; set; }
    
    public MaintenanceType MaintenanceType { get; set; }
}