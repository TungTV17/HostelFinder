using HostelFinder.Application.DTOs.Image.Requests;
using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.DTOs.Users.Response;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Response<UserDto>> RegisterUserAsync(CreateUserRequestDto request);
        Task<Response<List<UserDto>>> GetAllUsersAsync();
        Task <Response<UserProfileResponse>> GetUserByIdAsync(Guid id);
        Task<Response<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequestDto updateUserDto, UploadImageRequestDto? image);
        Task<Response<bool>> UnActiveUserAsync(Guid userId);
        Task<Response<UserProfileResponse>> GetUserByHostelIdAsync(Guid hostelId);
        Task<Response<string>> ManageUserMembershipAsync(Guid userId, Guid membershipId);
        Task<List<UserDto>> FilterUsersByActiveStatusAsync(bool isActive);
        
        //Upload QR Code
        Task<Response<string>> UploadQRCodeAsync(Guid userId, UploadQRCodeRequestDto request);
    }
}