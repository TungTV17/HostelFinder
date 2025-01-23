namespace HostelFinder.Application.DTOs.RentalContract.Request
{
    public class AddMeterReadingServiceDto
    {
        public int Reading { get; set; }

        public Guid ServiceId { get; set; }
    }
}
