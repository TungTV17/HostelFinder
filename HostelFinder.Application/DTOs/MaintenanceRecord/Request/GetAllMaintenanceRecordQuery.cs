using HostelFinder.Domain.Common.Constants;

namespace HostelFinder.Application.DTOs.MaintenanceRecord.Request;

public class GetAllMaintenanceRecordQuery
{
    public Guid HostelId { get; set; }
    public string? SearchPhrase { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}