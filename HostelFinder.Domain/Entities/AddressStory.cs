using HostelFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Domain.Entities
{
    public class AddressStory : BaseEntity
    {
        public Guid? StoryId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        [MaxLength(255)]
        public string DetailAddress { get; set; }
        public virtual Story? Story { get; set; }
    }
}