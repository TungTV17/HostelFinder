using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HostelFinder.Domain.Common;
using HostelFinder.Domain.Enums;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string ServiceName { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public ChargingMethod ChargingMethod { get; set; }

        //Navigation
        public ICollection<HostelServices> HostelServices { get; set; } 

        public virtual ICollection<ServiceCost> ServiceCosts { get; set; } 

        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } 

        public virtual ICollection<MeterReading> MeterReadings { get; set; } 


    }
}