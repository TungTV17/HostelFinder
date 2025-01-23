using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Invoice.Responses
{
    public class InvoiceDetailResponseDto
    {
        public Guid InvoiceId { get; set; }
        public string? ServiceName { get; set; }

        public decimal UnitCost { get; set; }

        public decimal ActualCost { get; set; }

        public int? NumberOfCustomer { get; set; }

        public int PreviousReading { get; set; }

        public int CurrentReading { get; set; }

        public bool IsRentRoom { get; set; }

        public DateTime BillingDate { get; set; }

    }
}
