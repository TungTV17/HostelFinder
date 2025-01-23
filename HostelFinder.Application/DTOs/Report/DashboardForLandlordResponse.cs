
namespace HostelFinder.Application.DTOs.Report
{
    public class DashboardForLandlordResponse
    {
        public int HostelCount { get; set; }
        public int TenantCount { get; set; }
        public int RoomCount { get; set; }
        public int OccupiedRoomCount { get; set; }  
        public int AvailableRoomCount { get; set; }
        public int AllInvoicesCount { get; set; } 
        public int UnpaidInvoicesCount { get; set; }
        public int ExpiringContractsCount { get; set; }
        
        public int PostCount { get; set; }
    }
}
