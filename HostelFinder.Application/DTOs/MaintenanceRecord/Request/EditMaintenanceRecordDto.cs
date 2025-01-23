using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.MaintenanceRecord.Request
{
    public class EditMaintenanceRecordDto
    {
        public Guid HostelId { get; set; }
        public Guid? RoomId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public decimal Cost { get; set; }
        public MaintenanceType MaintenanceType { get; set; }
    }

}
