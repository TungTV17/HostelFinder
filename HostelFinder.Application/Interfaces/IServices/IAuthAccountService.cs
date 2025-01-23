using HostelFinder.Application.DTOs.Auth.Requests;
using HostelFinder.Application.DTOs.Auth.Responses;
using HostelFinder.Application.DTOs.Auths.Requests;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IAuthAccountService
    {
        Task<Response<AuthenticationResponse>> LoginAsync(AuthenticationRequest request);

        Task<Response<string>> ChangePasswordAsync(ChangePasswordRequest request);

        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request);
        
        Task<Response<AuthenticationResponse>> LoginWithGoogleAsync(LoginWithGoogleRequest request);
        
    }
}