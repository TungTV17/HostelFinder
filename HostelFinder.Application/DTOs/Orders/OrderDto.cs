namespace HostelFinder.Application.DTOs.Orders
{
    public class OrderDto
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IpAddress { get; set; }
        public string? BankCode { get; set; }
    }
}
