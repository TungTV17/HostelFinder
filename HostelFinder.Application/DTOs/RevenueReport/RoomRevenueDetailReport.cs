namespace HostelFinder.Application.DTOs.RevenueReport;

public class RoomRevenueDetailReport
{
    public string RoomName { get; set; }
    public decimal TotalRevenue { get; set; }
    public int? Month { get; set; }
    public  int? Year { get; set; }
}