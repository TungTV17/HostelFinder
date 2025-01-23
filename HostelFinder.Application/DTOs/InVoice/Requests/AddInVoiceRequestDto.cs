namespace HostelFinder.Application.DTOs.InVoice.Requests
{
    public class AddInVoiceRequestDto
    {
        public Guid roomId { get; set; }
        public int billingMonth { get; set; }
        public int billingYear { get; set; }
    }
}
