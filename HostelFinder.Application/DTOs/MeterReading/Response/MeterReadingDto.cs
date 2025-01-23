
namespace HostelFinder.Application.DTOs.MeterReading.Response
{
    public class MeterReadingDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } // Thêm RoomName từ Navigation
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } // Thêm ServiceName từ Navigation
        public int Reading { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
    }
}
