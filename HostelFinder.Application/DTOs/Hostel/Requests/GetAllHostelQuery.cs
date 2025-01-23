using HostelFinder.Domain.Common.Constants;

namespace HostelFinder.Application.DTOs.Hostel.Requests
{
    public class GetAllHostelQuery
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
