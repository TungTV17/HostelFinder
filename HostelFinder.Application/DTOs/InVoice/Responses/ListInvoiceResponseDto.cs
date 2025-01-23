namespace HostelFinder.Application.DTOs.Invoice.Responses;

public class ListInvoiceResponseDto
{
    public Guid Id { get; set; }
    public string RoomName { get; set; }

    public int BillingMonth { get; set; }

    public int BillingYear { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }
}