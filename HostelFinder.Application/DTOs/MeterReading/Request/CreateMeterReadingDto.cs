namespace HostelFinder.Application.DTOs.MeterReading.Request
{
    public class CreateMeterReadingDto
    {
        public Guid roomId { get; set; }

        public Guid serviceId { get; set; }
        public int? previousReading { get; set; }

        public int currentReading {  get; set; }

        public int billingMonth { get; set; }

        public int billingYear { get; set;}
        
        
        
        
    }
    
}
