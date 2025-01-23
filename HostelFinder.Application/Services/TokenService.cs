using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoomFinder.Domain.Common.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace HostelFinder.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public TokenService(IOptions<JWTSettings> jwtSettings, IUserRepository userRepository, PasswordHasher<User> passwordHasher)
        {
            _jwtSettings = jwtSettings.Value;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public string GenerateJwtToken(User user, UserRole role)
        {
            var claims = new List<Claim>
            {
                new ("UserId", user.Id.ToString()),
                new (ClaimTypes.Name, user.Username),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.Role, role.ToString()),
                new ("Role", role.ToString()),
                new ("Username", user.Username),
            };

            // Generate signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            // Return serialized JWT
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        

        public async Task<string> GenerateNewPasswordRandom(User user)
        {
            try
            {
                var passwordBytes = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(passwordBytes);
                }
                var newPassword = Convert.ToBase64String(passwordBytes);
                user.Password = _passwordHasher.HashPassword(user, newPassword);
                await _userRepository.UpdateAsync(user);
                return newPassword;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}