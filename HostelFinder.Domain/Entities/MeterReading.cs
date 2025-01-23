using RoomFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class MeterReading : BaseEntity
    {
        [Required]
        [ForeignKey("Room")]
        public Guid RoomId { get; set; }

        [Required]
        [ForeignKey("Service")]
        public Guid ServiceId { get; set; }


        /// <summary>
        /// Số điện, số nước tại thời điểm đọc
        /// </summary>
        [Required]
        public int Reading { get; set; }

        [Required]
        public int BillingMonth {  get; set; }

        [Required]
        public int BillingYear { get; set; }

        //Navigation Properties

        public virtual Room Room { get; set; } 

        public virtual Service Service { get; set; } 

    }
}
