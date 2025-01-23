using HostelFinder.Domain.Common.Constants;

namespace HostelFinder.Application.DTOs.Post.Requests
{
    public class GetAllPostsQuery
    {
        public string? SearchPhrase { get; set; }   
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
