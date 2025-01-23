using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HostelFinder.Domain.Common;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities;

public class Image : BaseEntity
{
    [Required]
    public string Url { get; set; }

    [ForeignKey("Hostel")]
    public Guid? HostelId { get; set; }

    [ForeignKey("Post")]
    public Guid? PostId { get; set; }

    [ForeignKey("Room")]
    public Guid? RoomId { get; set; }

    [ForeignKey("Story")]
    public Guid? StoryId { get; set; }

    public virtual Hostel? Hostel   { get; set; }
    public virtual Post? Post   { get; set; }
    public virtual Story? Story   { get; set; }
    public virtual Room? Room { get; set; }
}