using HostelFinder.Application.Common;
using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IStoryRepository : IBaseGenericRepository<Story>
    {
        Task<Story> GetStoryByIdAsync(Guid id);
        Task<PagedResponse<List<Story>>> GetAllStoriesAsync(int pageIndex, int pageSize, StoryFilterDto filter);
        Task<PagedResponse<List<Story>>> GetAllStoriesNoCondition(int pageIndex, int pageSize);
        Task<PagedResponse<List<Story>>> GetStoriesByUserId(Guid userId, int pageIndex, int pageSize);
        Task<int> CountStoriesByUserTodayAsync(Guid userId);
    }
}
