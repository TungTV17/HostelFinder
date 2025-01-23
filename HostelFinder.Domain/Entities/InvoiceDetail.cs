using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class InvoiceDetail : BaseEntity
    {
        [ForeignKey("Invoice")]
        [Required]
        public Guid InvoiceId { get; set; }

        [ForeignKey("Service")]
        public Guid? ServiceId { get; set; }


        /// <summary>
        /// Giá tại thời điểm lập hóa đơn 
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; } 

        /// <summary>
        /// tổng chi phí cho dịch vụ
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualCost { get; set; }

        public int? NumberOfCustomer { get; set; }

        public int PreviousReading { get; set; }
        public int CurrentReading { get; set; }


        public bool IsRentRoom { get; set; }
        /// <summary>
        /// Ngày phát sinh hóa đơn
        /// </summary>
        [Required]
        public DateTime BillingDate { get; set; } 

        // Navigation
        public virtual Invoice Invoice { get; set; }
        public virtual Service? Service { get; set; }
    }
}
