using System.ComponentModel.DataAnnotations.Schema;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class RoomTenancy : BaseEntity
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        [ForeignKey("Room")]
        public Guid RoomId { get; set; }

        public DateTime MoveInDate { get; set; }
        
        public DateTime? MoveOutDate { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual Room Room { get; set; }
    }
}
