using HostelFinder.Domain.Common;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities;

public class Amenity : BaseEntity
{
    public string AmenityName { get; set; }
    public virtual ICollection<RoomAmenities> RoomAmenities { get; set; }
}