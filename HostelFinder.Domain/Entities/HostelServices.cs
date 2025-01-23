using HostelFinder.Domain.Common;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class HostelServices : BaseEntity
    {
        public Guid ServiceId { get; set; }
        
        public Guid HostelId { get; set; }

        public  Hostel Hostel { get; set; }

        public Service Services { get; set; }
    }
}
