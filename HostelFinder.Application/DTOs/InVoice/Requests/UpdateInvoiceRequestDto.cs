namespace HostelFinder.Application.DTOs.InVoice.Requests
{
    public class UpdateInvoiceRequestDto
    {
        public decimal TotalAmount { get; set; }
        public bool Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}
