namespace HostelFinder.Application.DTOs.Service.Response
{
    public class ServiceResponseDTO
    {
        public Guid Id { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public bool IsBillable { get; set; }
        public int Price { get; set; }
        public bool IsUsageBased { get; set; }
    }
}