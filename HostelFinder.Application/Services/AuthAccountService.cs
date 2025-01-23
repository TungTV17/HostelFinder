using Google.Apis.Auth;
using HostelFinder.Application.DTOs.Auth.Requests;
using HostelFinder.Application.DTOs.Auth.Responses;
using HostelFinder.Application.DTOs.Auths.Requests;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using HostelFinder.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace HostelFinder.Application.Services
{
    public class AuthAccountService : IAuthAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly INotificationRepository _notificationRepository;

        public AuthAccountService(IUserRepository userRepository
            , ITokenService tokenService,
            IEmailService emailService,
            INotificationRepository notificationRepository
            )
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _passwordHasher = new PasswordHasher<User>();
            _notificationRepository = notificationRepository;
        }

        public async Task<Response<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.FindByUserNameAsync(request.Username);
                if (user == null)
                {
                    return new Response<string> { Succeeded = false, Message = "Người dùng không tồn tại" };
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword);
                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return new Response<string> { Succeeded = false, Message = "Mật khẩu cũ không đúng" };
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return new Response<string> { Succeeded = false, Message = "Mật khẩu mới và xác nhận mật khẩu không giống nhau" };
                }

                if (string.Equals(request.CurrentPassword, request.NewPassword))
                {
                    return new Response<string> { Succeeded = false, Message = "Mật khẩu mới không thể trùng mật khẩu cũ" };
                }

                user.Password = _passwordHasher.HashPassword(user, request.NewPassword);
                await _userRepository.UpdateAsync(user);

                var notificationMessage = $"{user.Username} đã thay đổi mật khẩu.";
                var notification = new Notification
                {
                    Message = notificationMessage,
                    UserId = user.Id,
                    CreatedOn = DateTime.Now
                };
                await _notificationRepository.AddAsync(notification);
                return new Response<string> { Succeeded = true, Message = "Đổi mật khẩu thành công" };
            }
            catch (Exception ex)
            {
                return new Response<string> { Succeeded = false, Message = $"Error: {ex.Message}" };
            }
        }


        public async Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userRepository.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false, Message = "Email không tồn tại. Vui lòng kiểm tra hoặc tạo tài khoản mới."
                    };
                }

                var newPassword = await _tokenService.GenerateNewPasswordRandom(user);

                var emailBody = EmailConstants.BodyResetPasswordEmail(user,user.Email, newPassword);
                var emailSubject = "Mật khẩu mới của bạn";
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                var notificationMessage = $"{user.Username} đã yêu cầu lấy lại mật khẩu.";
                var notification = new Notification
                {
                    Message = notificationMessage,
                    UserId = user.Id, 
                    CreatedOn = DateTime.Now
                };
                await _notificationRepository.AddAsync(notification);
                return new Response<string>
                {
                    Succeeded = true,
                    Message = "Mật khẩu mới gửi tới email của bạn. Vui lòng check email!"
                };
            }
            catch (Exception ex)
            {
                return new Response<string>{Succeeded = false, Message = ex.Message};
            }
        }

        public async Task<Response<AuthenticationResponse>> LoginAsync(AuthenticationRequest request)
        {
            try
            {
                var user = await _userRepository.FindByUserNameAsync(request.UserName);
                if (user == null)
                {
                    return new Response<AuthenticationResponse> { Succeeded = false, Message = "Tên người dùng không tồn tại. Vui lòng kiểm tra hoặc tạo tài khoản mới." };
                }

                if (user.IsActive == false)
                {
                    return new Response<AuthenticationResponse>()
                    {
                        Succeeded = false,
                        Message =
                            "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên để biêt thêm chi tiết"
                    };
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return new Response<AuthenticationResponse> { Succeeded = false, Message = "Tài khoản hoặc mật khẩu không đúng. Vui lòng kiểm tra lại!" };
                }

                var role = await _userRepository.GetRoleAsync(user.Id);
                var token = _tokenService.GenerateJwtToken(user, role);

                var response = new AuthenticationResponse
                {
                    UserName = user.Username,
                    Role = role.ToString(),
                    Token = token
                };

                var notificationMessage = $"Chào mừng {user.Username} đến với PhongTro247!!!";
                var notification = new Notification
                {
                    Message = notificationMessage,
                    UserId = user.Id,
                    CreatedOn = DateTime.Now
                };
                await _notificationRepository.AddAsync(notification);
                return new Response<AuthenticationResponse> { Data = response, Succeeded = true, Message = "Đăng nhập thành công" };
            }
            catch (Exception ex)
            {
                return new Response<AuthenticationResponse> { Message = ex.Message };
            }
        }

        public async Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new Response<string> { Succeeded = false, Message = "Email không tồn tại. Vui lòng kiểm tra hoặc tạo tài khoản mới." };
            }
            

            user.Password = _passwordHasher.HashPassword(user, request.NewPassword);

            await _userRepository.UpdateAsync(user);

            return new Response<string> { Succeeded = true, Message = "Đặt lại mật khẩu thành công!" };
        }

        public async Task<Response<AuthenticationResponse>> LoginWithGoogleAsync(LoginWithGoogleRequest request)
        {
            try
            {
                // Logging để kiểm tra idToken
                Console.WriteLine($"Received idToken: {request.IdToken}");

                var googleUser = await AuthenticateWithGoogle(request.IdToken);
                if (googleUser == null)
                    throw new Exception("Invalid Google Token.");

                var user = await _userRepository.FindByEmailAsync(googleUser.Email);
                if (user == null)
                {
                    user = new User
                    {
                        FullName = googleUser.Name,
                        Email = googleUser.Email,
                        AvatarUrl = "https://hostel-finder-images.s3.ap-southeast-1.amazonaws.com/Default-Avatar.png",
                        Role = UserRole.User, 
                        IsActive = true,
                        IsEmailConfirmed = true,
                        WalletBalance = 0,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "Google",
                        Username = googleUser.Name
                    };

                    await _userRepository.AddAsync(user);
                }
                var role = await _userRepository.GetRoleAsync(user.Id);
                var token =  _tokenService.GenerateJwtToken(user, role);
                var response = new AuthenticationResponse
                {
                    UserName = user.FullName,
                    Role = role.ToString(),
                    Token = token
                };

                var notificationMessage = $"Chào mừng {user.Username} đến với PhongTro247!!!";
                var notification = new Notification
                {
                    Message = notificationMessage,
                    UserId = user.Id,
                    CreatedOn = DateTime.Now
                };
                await _notificationRepository.AddAsync(notification);
                return new Response<AuthenticationResponse> { Data = response, Succeeded = true, Message = "Đăng nhập thành công" };
                
            }
            catch (Exception ex)
            {
                // Logging lỗi để dễ dàng kiểm tra
                Console.WriteLine($"Error in GoogleLogin: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }
        private async Task<GoogleJsonWebSignature.Payload> AuthenticateWithGoogle(string idToken)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json")
                    .Build();
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { configuration["Google:ClientId"] }
                });
                return payload;
            }
            catch (Exception ex)
            {
                // Logging để kiểm tra lỗi xác thực
                Console.WriteLine($"Invalid JWT: {ex.Message}");
                return null;
            }
        }
    }
}