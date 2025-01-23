using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities;

public class Address : BaseEntity
{
    public Guid? HostelId { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public string Commune { get; set; }
    [MaxLength(255)]
    public string DetailAddress { get; set; }
    public virtual Hostel? Hostel { get; set; }
}