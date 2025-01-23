using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IImageRepository : IBaseGenericRepository<Image>
    {
        Task<Image> GetImageUrlByRoomId(Guid roomId);
        Task<List<Image>> GetImagesByHostelIdAsync(Guid hostelId);
        Task<List<Image>> GetImagesByPostIdAsync(Guid postId);
        Task<List<Image>> GetImagesByStoryIdAsync(Guid storyId);
        Task<List<string>> GetAllUrlRoomPicture(Guid roomId);
        Task DeleteByRoomId(Guid roomId);
    }
}
