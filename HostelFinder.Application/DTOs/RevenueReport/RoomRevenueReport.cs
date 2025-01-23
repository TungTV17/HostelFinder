namespace HostelFinder.Application.DTOs.RevenueReport;

public class RoomRevenueReport
{
    public decimal TotalAllRevenue { get; set; }
    public decimal? TotalRoomRevenue { get; set; }
    public decimal? TotalCostOfMaintenance { get; set; }
    public List<RoomRevenueDetailReport>? RoomRevenueDetail { get; set; }
}