
using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.DTOs.Story.Responses;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IStoryService
    {
        Task<Response<string>> AddStoryAsync(AddStoryRequestDto request);
        Task<Response<StoryResponseDto>> GetStoryByIdAsync(Guid id);
        Task<PagedResponse<List<ListStoryResponseDto>>> GetAllStoriesAsync(int pageIndex, int pageSize, StoryFilterDto filter);
        Task<PagedResponse<List<ListStoryResponseDto>>> GetAllStoryForAdminAsync(int pageIndex, int pageSize);
        Task<PagedResponse<List<ListStoryResponseDto>>> GetStoryByUserIdAsync(Guid userId, int pageIndex, int pageSize);
        Task<Response<string>> DeleteStoryAsync(Guid storyId);
        Task<Response<StoryResponseDto>> UpdateStoryAsync(Guid storyId, UpdateStoryRequestDto request, List<IFormFile>? images, List<string>? imageUrls);
        Task<Response<StoryResponseDto>> AcceptStoryAsync(Guid storyId);
        Task<Response<StoryResponseDto>> DenyStoryAsync(Guid storyId);
    }
}
