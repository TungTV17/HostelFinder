using RoomFinder.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        [ForeignKey("Room")]
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public int BillingMonth { get; set; }

        [Required]

        public int BillingYear { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount{ get; set; }

        public bool IsPaid { get; set; } = false;
        
        public string? FormOfTransfer { get; set; }
        
        public decimal? AmountPaid { get; set; }
        
        //Navigation
        public virtual Room Room { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();


    }
}