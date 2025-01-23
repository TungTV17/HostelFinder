using AutoMapper;
using FluentValidation;
using HostelFinder.Application.DTOs.Image.Requests;
using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.DTOs.Users.Response;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using HostelFinder.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HostelFinder.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserMembershipRepository _userMembershipRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IValidator<CreateUserRequestDto> _createUserValidator;
        private readonly IS3Service _s3Service;
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;

        public UserService
        (
            IMapper mapper,
            IUserRepository userRepository,
            IMembershipRepository membershipRepository,
            IValidator<CreateUserRequestDto> createUserValidator,
            IS3Service s3Service,
            IUserMembershipRepository userMembershipRepository,
            IEmailService emailService,
            INotificationRepository notificationRepository
        )
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _createUserValidator = createUserValidator;
            _s3Service = s3Service;
            _membershipRepository = membershipRepository;
            _userMembershipRepository = userMembershipRepository;
            _emailService = emailService;
            _notificationRepository = notificationRepository;
        }

        public async Task<Response<UserDto>> RegisterUserAsync(CreateUserRequestDto request)
        {
            var validationResult = await _createUserValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return new Response<UserDto> { Succeeded = false, Errors = errors };
            }

            try
            {
                if (await _userRepository.CheckUserNameExistAsync(request.Username))
                {
                    return new Response<UserDto>
                    { Succeeded = false, Message = "Tên người dùng đã tồn tại. Vui lòng nhập tên khác" };
                }

                if (await _userRepository.CheckEmailExistAsync(request.Email))
                {
                    return new Response<UserDto>
                    { Succeeded = false, Message = "Email đã tồn tại. Vui lòng nhập email khác." };
                }

                if (await _userRepository.CheckPhoneNumberAsync(request.Phone))
                {
                    return new Response<UserDto>
                    { Succeeded = false, Message = "Số điện thoại đã tồn tại. Vui lòng nhập số điện thoại khác." };
                }

                var userDomain = _mapper.Map<User>(request);

                userDomain.Password = _passwordHasher.HashPassword(userDomain, userDomain.Password);
                userDomain.Role = UserRole.User;
                userDomain.IsEmailConfirmed = false;
                userDomain.AvatarUrl =
                    "https://hostel-finder-images.s3.ap-southeast-1.amazonaws.com/Default-Avatar.png";
                userDomain.IsDeleted = false;
                userDomain.CreatedOn = DateTime.Now;

                var user = await _userRepository.AddAsync(userDomain);             
                var userDto = _mapper.Map<UserDto>(user);

                return new Response<UserDto>
                { Succeeded = true, Data = userDto, Message = "Bạn đã đăng ký thành công tài khoản" };
            }
            catch (Exception ex)
            {
                return new Response<UserDto> { Succeeded = false, Errors = { ex.Message } };
            }
        }

        public async Task<Response<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return new Response<List<UserDto>>
                { Succeeded = false, Errors = new List<string> { "No users found." } };
            }

            var userDtos = _mapper.Map<List<UserDto>>(users);
            return new Response<List<UserDto>> { Data = userDtos, Succeeded = true };
        }

        public async Task<Response<UserProfileResponse>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new Response<UserProfileResponse>
                { Succeeded = false, Errors = new List<string> { "User not found." } };
            }

            var userProfileResponse = _mapper.Map<UserProfileResponse>(user);

            return new Response<UserProfileResponse> { Data = userProfileResponse, Succeeded = true };
        }

        public async Task<Response<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequestDto updateUserDto,
            UploadImageRequestDto? image)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new Response<UserDto>("Người dùng không tồn tại.");
            }

            if (image.Image != null)
            {
                var imageUrl = await _s3Service.UploadFileAsync(image.Image);
                user.AvatarUrl = imageUrl;
            }

            user.Username = updateUserDto.Username;
            user.Email = updateUserDto.Email;
            user.Phone = updateUserDto.Phone;
            user.FullName = updateUserDto.FullName;

            await _userRepository.UpdateAsync(user);

            var notificationMessage = $"{updateUserDto.Username} đã cập nhật thông tin cá nhân.";
            var notification = new Notification
            {
                Message = notificationMessage,
                UserId = user.Id,
                CreatedOn = DateTime.Now
            };
            await _notificationRepository.AddAsync(notification);
            var updatedUserDto = _mapper.Map<UserDto>(user);
            return new Response<UserDto>(updatedUserDto);
        }

        public async Task<Response<bool>> UnActiveUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new Response<bool>("Người dùng không tồn tại.");
            }

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return new Response<bool>(true);
        }

        public async Task<Response<UserProfileResponse>> GetUserByHostelIdAsync(Guid hostelId)
        {
            var user = await _userRepository.GetUserByHostelIdAsync(hostelId);
            var userDto = _mapper.Map<UserProfileResponse>(user);
            return new Response<UserProfileResponse>(userDto);
        }

        public async Task<Response<string>> ManageUserMembershipAsync(Guid userId, Guid membershipId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return CreateErrorResponse("User not found.");
            }

            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                return CreateErrorResponse("Membership not found.");
            }

            var userMemberships = await _userMembershipRepository.GetByUserIdAsync(userId);
            bool isTrial = membership.Duration <= 7;
            // Kiểm tra nếu người dùng đã có gói thành viên nào, nếu có, tìm gói tương ứng
            var existingUserMembership = userMemberships.FirstOrDefault(um => um.MembershipId == membershipId && !um.IsDeleted);
            if (existingUserMembership != null)
            {
                return await HandleExistingMembership(user, existingUserMembership, membership, isTrial);
            }

            return await HandleNewMembership(user, membership, isTrial);
        }

        private async Task<Response<string>> HandleExistingMembership(User user, UserMembership userMembership, Membership membership, bool isTrial)
        {
            // Xử lý gói thử nghiệm
            if (isTrial)
            {
                var existingTrialMembership = await _userMembershipRepository.GetTrialMembershipByUserIdAsync(user.Id);
                if (existingTrialMembership != null)
                {
                    if (existingTrialMembership.ExpiryDate > DateTime.Now)
                    {
                        return CreateErrorResponse("Bạn đã sử dụng gói người dùng thử này và chưa hết hạn.");
                    }
                    else
                    {
                        return CreateErrorResponse("Gói người dùng thử đã hết hạn.");
                    }
                }
                return await RegisterTrialMembership(user.Id, membership.Id, membership.Duration);
            }

            // Xử lý gói trả phí
            if (userMembership.MembershipId == membership.Id && userMembership.ExpiryDate > DateTime.Now)
            {
                return CreateErrorResponse("Bạn đang sử dụng gói thành viên này. Không thể mua thêm!");
            }
            
            // Kiểm tra ví và trừ tiền nếu là gói trả phí
            return await HandlePaidMembership(user, membership);
        }

        private async Task<Response<string>> HandleNewMembership(User user, Membership membership, bool isTrial)
        {
            if (isTrial) // Đăng ký gói thử nghiệm cho người dùng mới
            {
                return await RegisterTrialMembership(user.Id, membership.Id, membership.Duration);
            }

            // Kiểm tra ví và trừ tiền nếu là gói trả phí
            return await HandlePaidMembership(user, membership);
        }

        private async Task<Response<string>> HandlePaidMembership(User user, Membership membership)
        {
            if (user.WalletBalance < membership.Price)
            {
                return CreateErrorResponse("Số dư không đủ! Vui lòng nạp tiền thêm vào ví.");
            }

            user.WalletBalance -= membership.Price;
            await _userRepository.UpdateAsync(user);

            var emailBody = EmailConstants.BodyRegisterMembership(user, membership);
            var emailSubject = EmailConstants.SUBJECT_REGISTER_MEMBERSHIP;
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            // Tạo hoặc cập nhật membership mới
            await CreateOrUpdateMembership(user.Id, membership.Id, membership.Duration);

            if (user.Role == UserRole.User)
            {
                user.Role = UserRole.Landlord;
                await _userRepository.UpdateAsync(user);
            }

            var notificationMessage = "Gói thành viên mới đã đăng ký thành công.";
            var notification = new Notification
            {
                Message = notificationMessage,
                UserId = user.Id,
                CreatedOn = DateTime.Now
            };
            await _notificationRepository.AddAsync(notification);
            return CreateSuccessResponse("Gói thành viên mới đã đăng ký thành công.");
        }

        private async Task<Response<string>> RegisterTrialMembership(Guid userId, Guid membershipId, int duration)
        {
            // Không cần trừ tiền nếu là gói thử nghiệm
            var startDate = DateTime.Now;
            var expiryDate = startDate.AddDays(duration);
            var newUserMembership = new UserMembership
            {
                UserId = userId,
                MembershipId = membershipId,
                StartDate = startDate,
                ExpiryDate = expiryDate,
                PostsUsed = 0,
                PushTopUsed = 0,
                IsPaid = true, 
                CreatedBy = "System",
                CreatedOn = DateTime.Now
            };
            await _userMembershipRepository.AddAsync(newUserMembership);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null && user.Role == UserRole.User)
            {
                user.Role = UserRole.Landlord;
                await _userRepository.UpdateAsync(user);
            }
            var notificationMessage = "Bạn đã đăng ký gói người dùng thử thành công.";
            var notification = new Notification
            {
                Message = notificationMessage,
                UserId = user.Id,
                CreatedOn = DateTime.Now
            };
            await _notificationRepository.AddAsync(notification);
            return CreateSuccessResponse("Gói người dùng thử đã đăng ký thành công.");
        }

        private Response<string> CreateErrorResponse(string message)
        {
            return new Response<string>
            {
                Succeeded = false,
                Message = message
            };
        }

        private Response<string> CreateSuccessResponse(string message)
        {
            return new Response<string>
            {
                Succeeded = true,
                Message = message
            };
        }

        private async Task CreateOrUpdateMembership(Guid userId, Guid membershipId, int duration)
        {
            var startDate = DateTime.Now;
            var expiryDate = startDate.AddDays(duration);

            var existingMembership = await _userMembershipRepository.GetByUserIdAndMembershipIdAsync(userId, membershipId);
            if (existingMembership != null)
            {
                existingMembership.ExpiryDate = expiryDate;
                existingMembership.PostsUsed = 0; 
                existingMembership.PushTopUsed = 0; 
                await _userMembershipRepository.UpdateAsync(existingMembership);
            }
            else
            {
                var newUserMembership = new UserMembership
                {
                    UserId = userId,
                    MembershipId = membershipId,
                    StartDate = startDate,
                    ExpiryDate = expiryDate,
                    PostsUsed = 0,
                    PushTopUsed = 0,
                    IsPaid = true,  
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now
                };
                await _userMembershipRepository.AddAsync(newUserMembership);
            }
        }

        public async Task<List<UserDto>> FilterUsersByActiveStatusAsync(bool isActive)
        {
            var users = await _userRepository.FilterUsersByActiveStatusAsync(isActive);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<Response<string>> UploadQRCodeAsync(Guid userId, UploadQRCodeRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new Response<string>() { Succeeded = false, Message = "Không tìm thấy người dùng." };
                }

                user.QRCode = await _s3Service.UploadFileAsync(request.QRCodeImage);
                user.BankName = request.BankName;
                user.AccountNumber = request.AccountNumber;
                user.LastModifiedOn = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                return new Response<string>() { Succeeded = true, Message = "Cập nhật QR Code thành công" };
            }
            catch (Exception ex)
            {
                return new Response<string>() { Succeeded = false, Message = ex.Message };
            }
        }
    }
}