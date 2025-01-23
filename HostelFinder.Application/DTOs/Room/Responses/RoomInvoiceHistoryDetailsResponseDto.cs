using HostelFinder.Application.DTOs.Invoice.Responses;

namespace HostelFinder.Application.DTOs.Room.Responses
{
    public class RoomInvoiceHistoryDetailsResponseDto
    {
        public Guid Id { get; set; }

        public int BillingMonth { get; set; }

        public int BillingYear { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; } = false;

        public virtual ICollection<InvoiceDetailResponseDto?>? InvoiceDetails { get; set; }
    }
}
