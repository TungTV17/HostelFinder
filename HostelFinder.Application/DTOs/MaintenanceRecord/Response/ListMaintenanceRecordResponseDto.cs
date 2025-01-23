using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.MaintenanceRecord.Response;

public class ListMaintenanceRecordResponseDto
{
    public Guid Id { get; set; }
    public Guid HostelId { get; set; }
    public Guid? RoomId { get; set; }
    public string HostelName { get; set; }
    public string? RoomName { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public decimal Cost { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
}