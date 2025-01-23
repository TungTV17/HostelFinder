using HostelFinder.Application.Common;
using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage;

namespace HostelFinder.Application.Interfaces.IRepositories;

public interface IPostRepository : IBaseGenericRepository<Post>
{
    Task<IEnumerable<Post>> GetPostsByUserIdAsync(Guid userId);

    Task<(IEnumerable<Post> Data, int TotalRecords)> GetAllMatchingAsync(string? searchPhrase, int pageSize,
        int pageNumber, string? sortBy, SortDirection sortDirection);

    Task<Post?> GetPostByIdAsync(Guid postId);
    Task<Post?> GetPostByIdWithHostelAsync(Guid postId);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<List<Post>> FilterPostsAsync(string? province, string? district, string? commune, decimal? minSize,
        decimal? maxSize, decimal? minPrice, decimal? maxPrice, RoomType? roomType);
    Task<List<Post>> GetPostsOrderedByMembershipPriceAndCreatedOnAsync();
    Task<List<Post>> GetAllPostsOrderedAsync();
    Task<PagedResponse<List<Post>>> GetAllPostsOrderedAsync(int pageIndex, int pageSize);
    Task<PagedResponse<List<Post>>> GetPostsOrderedByMembershipPriceAndCreatedOnAsync(int pageIndex, int pageSize); 
    Task<PagedResponse<List<Post>>> GetFilteredAndPagedPostsAsync(FilterPostsRequestDto filter, int pageIndex, int pageSize);
    Task<List<Post>> GetTopPostsAsync(int topCount);
    Task<bool> CheckUserHostelExist(Guid userId);
}