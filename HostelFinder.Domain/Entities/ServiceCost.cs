using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HostelFinder.Domain.Common;
using HostelFinder.Domain.Enums;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities;

public class ServiceCost : BaseEntity
{
    [ForeignKey("Hostel")] 
    [Required] 
    public Guid HostelId { get; set; }
    [ForeignKey("Service")]
    [Required]
    public Guid ServiceId { get; set; }
    [Required]
    [Column(TypeName ="decimal(18,2)")]
    
    public decimal UnitCost { get; set; }

    public UnitType Unit {  get; set; }

    [Required]
    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }



    //navigation
    public virtual Hostel Hostel { get; set; }  
    public virtual Service Service { get; set; }
}