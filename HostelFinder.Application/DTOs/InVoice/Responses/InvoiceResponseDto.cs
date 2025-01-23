using HostelFinder.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Application.DTOs.Invoice.Responses;

namespace HostelFinder.Application.DTOs.InVoice.Responses
{
    public class InvoiceResponseDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string? RoomName { get; set; }

        public int BillingMonth { get; set; }

        public int BillingYear { get; set; }

        public decimal TotalAmount { get; set; }
        
        public string? FormOfTransfer { get; set; }
        
        public decimal? AmountPaid { get; set; }

        public bool IsPaid { get; set; } = false;

        public virtual ICollection<InvoiceDetailResponseDto> InvoiceDetails { get; set; } = new List<InvoiceDetailResponseDto>();
    }
}
