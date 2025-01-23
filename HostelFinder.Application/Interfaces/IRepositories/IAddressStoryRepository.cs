using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IAddressStoryRepository : IBaseGenericRepository<AddressStory>
    {
        Task<AddressStory> GetAddressByStoryIdAsync(Guid storyId);
    }
}
