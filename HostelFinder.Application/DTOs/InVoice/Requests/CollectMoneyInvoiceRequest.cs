namespace HostelFinder.Application.DTOs.InVoice.Requests;

public class CollectMoneyInvoiceRequest
{
    public Guid InvoiceId { get; set; }
    public string? FormOfTransfer { get; set; }
    public decimal? AmountPaid { get; set; }
    public DateTime DateOfSubmit { get; set; }
}