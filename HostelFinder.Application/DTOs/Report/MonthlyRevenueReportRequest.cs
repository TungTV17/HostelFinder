namespace HostelFinder.Application.DTOs.Report;

public class MonthlyRevenueReportRequest
{
    public Guid HostelId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}