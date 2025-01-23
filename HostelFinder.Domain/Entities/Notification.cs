
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
