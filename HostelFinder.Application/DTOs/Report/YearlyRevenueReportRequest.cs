namespace HostelFinder.Application.DTOs.Report;

public class YearlyRevenueReportRequest
{
    public Guid HostelId { get; set; }
    public int Year { get; set; }
}