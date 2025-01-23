using HostelFinder.Application.DTOs.RoomTenancies.Request;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IRoomTenancyService
    {
        Task<Response<string>> AddTenantToRoomAsync(AddRoomTenacyDto request);
    }
}
