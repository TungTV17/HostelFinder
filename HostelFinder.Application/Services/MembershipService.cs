using AutoMapper;
using HostelFinder.Application.DTOs.Membership.Requests;
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.DTOs.MembershipService.Requests;
using HostelFinder.Application.DTOs.MembershipService.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserMembershipRepository _userMembershipRepository;
        private readonly IMapper _mapper;

        public MembershipService(IMembershipRepository membershipRepository, IMapper mapper,
            IUserMembershipRepository userMembershipRepository, IUserRepository userRepository)
        {
            _membershipRepository = membershipRepository;
            _mapper = mapper;
            _userMembershipRepository = userMembershipRepository;
            _userRepository = userRepository;
        }

        public async Task<Response<List<MembershipResponseDto>>> GetAllMembershipWithMembershipService()
        {
            var memberships = await _membershipRepository.GetAllMembershipWithMembershipService();

            if (memberships == null || !memberships.Any())
            {
                return new Response<List<MembershipResponseDto>>
                {
                    Succeeded = false,
                    Errors = new List<string> { "No memberships found." }
                };
            }

            var response = new Response<List<MembershipResponseDto>>
            {
                Data = _mapper.Map<List<MembershipResponseDto>>(memberships),
                Succeeded = true
            };

            return response;
        }


        public async Task<Response<MembershipResponseDto>> AddMembershipAsync(AddMembershipRequestDto membershipDto)
        {
            var membership = _mapper.Map<Membership>(membershipDto);

            membership.CreatedOn = DateTime.Now;
            membership.CreatedBy = "System";

            try
            {
                var membershipServiceRequests =
                    _mapper.Map<AddMembershipServiceReqDto>(membershipDto.MembershipServices);

                await _membershipRepository.AddMembershipWithServiceAsync(membership, membershipServiceRequests);

                var membershipResponseDto = _mapper.Map<MembershipResponseDto>(membership);
                membershipResponseDto.MembershipServices =
                    _mapper.Map<MembershipServiceResponseDto>(membership.MembershipServices);

                return new Response<MembershipResponseDto>
                {
                    Data = membershipResponseDto,
                    Message = "Gói thành viện đã tạo thành công!"
                };
            }
            catch (Exception ex)
            {
                return new Response<MembershipResponseDto>(message: ex.Message);
            }
        }

        public async Task<Response<MembershipResponseDto>> EditMembershipAsync(Guid id, UpdateMembershipRequestDto membershipDto)
        {
            var membership = await _membershipRepository.GetMembershipWithServicesAsync(id);
            if (membership == null)
            {
                return new Response<MembershipResponseDto>("Membership not found.");
            }

            _mapper.Map(membershipDto, membership);
            membership.LastModifiedOn = DateTime.Now;
            membership.LastModifiedBy = "System";

            var existingService = membership.MembershipServices.FirstOrDefault();

            if (existingService != null)
            {
                existingService.ServiceName = membershipDto.MembershipServices.ServiceName;
                existingService.MaxPostsAllowed = membershipDto.MembershipServices.MaxPostsAllowed;
                existingService.MaxPushTopAllowed = membershipDto.MembershipServices.MaxPushTopAllowed;
                existingService.LastModifiedOn = DateTime.Now;
                existingService.LastModifiedBy = "System";
            }
            else
            {
                return new Response<MembershipResponseDto>("No existing service found to update.");
            }

            await _membershipRepository.UpdateAsync(membership);

            var membershipResponseDto = _mapper.Map<MembershipResponseDto>(membership);

            return new Response<MembershipResponseDto>
            {
                Data = membershipResponseDto,
                Message = "Gói thành viên được cập nhật thành công!"
            };
        }

        public async Task<Response<bool>> DeleteMembershipAsync(Guid id)
        {
            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                return new Response<bool>(false, "Membership not found.");
            }

            // Kiểm tra xem có user nào đang sử dụng membership này và chưa hết hạn không
            var activeUserMemberships = await _userMembershipRepository.GetActiveUserMembershipsByMembershipIdAsync(id);
            if (activeUserMemberships.Any())
            {
                return new Response<bool>("Có người dùng đang sử dụng gói thành viên này, không thể xóa!");
            }

            var membershipServices = await _membershipRepository.GetMembershipServicesByMembershipIdAsync(id);
            if (membershipServices != null)
            {
                foreach (var service in membershipServices)
                {
                    await _membershipRepository.DeleteAsync(service.Id);
                }
            }

            await _membershipRepository.DeleteAsync(membership.Id);
            return new Response<bool>(true, "Membership deleted successfully.");
        }

        public async Task<Response<string>> UpdatePostCountAsync(Guid userId)
        {
            var userMembership = await _userMembershipRepository.GetUserMembershipByUserIdAsync(userId);
            if (userMembership != null && userMembership.Membership != null)
            {
                var membershipService = userMembership.Membership.MembershipServices
                    .FirstOrDefault(ms => ms.MaxPostsAllowed > userMembership.PostsUsed);

                if (membershipService != null && userMembership.PostsUsed < membershipService.MaxPostsAllowed)
                {
                    userMembership.PostsUsed++;
                    await _userMembershipRepository.UpdateAsync(userMembership);

                    return new Response<string>
                    {
                        Succeeded = true,
                        Message = "Post count updated successfully."
                    };
                }
                else
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Bạn đã dùng hết lượt đăng bài cho gói hội viên này."
                    };
                }
            }

            return new Response<string>
            {
                Succeeded = false,
                Message = "User membership not found."
            };
        }

        public async Task<Response<string>> UpdatePushTopCountAsync(Guid userId)
        {
            var userMembership = await _userMembershipRepository.GetUserMembershipByUserIdAsync(userId);
            if (userMembership != null && userMembership.Membership != null)
            {
                // Find the membership service that supports push-to-top functionality
                var membershipService = userMembership.Membership.MembershipServices
                    .FirstOrDefault(ms =>
                        ms.MaxPushTopAllowed.HasValue && ms.MaxPushTopAllowed > userMembership.PushTopUsed);

                if (membershipService != null && userMembership.PushTopUsed < membershipService.MaxPushTopAllowed)
                {
                    // Increment the push count for the user
                    userMembership.PushTopUsed++;
                    await _userMembershipRepository.UpdateAsync(userMembership);

                    return new Response<string>
                    {
                        Succeeded = true,
                        Message = "Push-to-top count updated successfully."
                    };
                }
                else
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message =
                            "You have reached the maximum number of push-to-top actions allowed for your membership."
                    };
                }
            }

            return new Response<string>
            {
                Succeeded = false,
                Message = "User membership not found."
            };
        }

        public async Task<Response<List<PostingMemberShipServiceDto>>> GetMembershipServicesForUserAsync(Guid userId)
        {
            try
            {
                // Lấy MembershipServices từ Repository
                var membershipServices = await _membershipRepository.GetMembershipServicesByUserAsync(userId);

                if (membershipServices == null || !membershipServices.Any())
                {
                    return new Response<List<PostingMemberShipServiceDto>>
                    {
                        Succeeded = false,
                        Message = "No membership services found for this user."
                    };
                }

                // Ánh xạ sang DTO
                var postingMemberShipServiceDtos = _mapper.Map<List<PostingMemberShipServiceDto>>(membershipServices);

                // Tính toán số bài đăng còn lại
                foreach (var service in membershipServices)
                {
                    // Kiểm tra Membership của service và tìm UserMembership tương ứng
                    var userMembership = service.Membership?.UserMemberships?.FirstOrDefault(um => um.UserId == userId);
                    if (userMembership != null)
                    {
                        // Tìm kiếm DTO tương ứng với service hiện tại
                        var dto = postingMemberShipServiceDtos.FirstOrDefault(d => d.Id == service.Id);
                        if (dto != null)
                        {
                            // Tính toán số bài đăng còn lại
                            int maxPostsAllowed = service.MaxPostsAllowed ?? 0; // Nếu MaxPostsAllowed là null, sử dụng 0
                            int postsUsed = userMembership.PostsUsed > 0 ? userMembership.PostsUsed : 0;
                            dto.NumberOfPostsRemaining = maxPostsAllowed - postsUsed;
                            // Tính toán số lần push top còn lại
                            int maxPushTopAllowed = service.MaxPushTopAllowed ?? 0; // Nếu MaxPushTopAllowed là null, sử dụng 0
                            int pushTopUsed = userMembership.PushTopUsed > 0 ? userMembership.PushTopUsed : 0;
                            dto.NumberOfPushTopRemaining = maxPushTopAllowed - pushTopUsed;
                        }
                    }
                }

                return new Response<List<PostingMemberShipServiceDto>>
                {
                    Succeeded = true,
                    Data = postingMemberShipServiceDtos,
                    Message = "Membership services fetched successfully."
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PostingMemberShipServiceDto>>
                {
                    Succeeded = false,
                    Message = "Internal server error: " + ex.Message
                };
            }
        }
    }
}