using HostelFinder.Application.DTOs.Room;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IRoomService
    {
        Task<Response<List<RoomResponseDto>>> GetAllAsync();
        Task<Response<RoomByIdDto>> GetByIdAsync(Guid id);
        Task<Response<RoomResponseDto>> CreateRoomAsync(AddRoomRequestDto roomDto, List<IFormFile> roomImages);
        Task<Response<RoomResponseDto>> UpdateAsync(Guid id, UpdateRoomRequestDto roomDto, List<IFormFile> roomImages);
        Task<Response<bool>> DeleteAsync(Guid id);
        Task<Response<List<RoomResponseDto>>> GetRoomsByHostelIdAsync(Guid hostelId,int? floor);

        Task<Response<List<RoomServiceDto>>> GetServicesByRoom(Guid roomId);
        Task<Response<GetAllInformationRoomResponseDto>> GetInformationDetailRoom(Guid roomId);

        Task<Response<EditRoomDtoResponse>> GetRoomWithAmentitesAndImageAsync(Guid roomId);

        Task<Response<bool>> CheckDeleteRoom(Guid roomId);
        
        Task<Response<List<SelectRoomResponse>>> GetSelectRoomByHostelAsync(Guid hostelId);

    }
}
