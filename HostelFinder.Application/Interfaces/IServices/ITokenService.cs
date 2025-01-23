using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, UserRole role);
        
        int? ValidateToken(string token);
        
        Task<string> GenerateNewPasswordRandom(User user);
    }
}