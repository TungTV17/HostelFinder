using AutoMapper;
using HostelFinder.Application.DTOs.Amenity.Request;
using HostelFinder.Application.DTOs.Amenity.Response;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.DTOs.RoomDetails.Response;
using HostelFinder.Application.DTOs.ServiceCost.Request;
using HostelFinder.Application.DTOs.ServiceCost.Responses;
using HostelFinder.Domain.Entities;
using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.DTOs.Hostel.Requests;
using HostelFinder.Application.DTOs.Hostel.Responses;
using HostelFinder.Application.DTOs.RoomDetails.Request;
using HostelFinder.Application.DTOs.Address;
using HostelFinder.Application.DTOs.Service.Request;
using HostelFinder.Application.DTOs.Service.Response;
using HostelFinder.Application.DTOs.Users.Response;
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.DTOs.Membership.Requests;
using HostelFinder.Application.DTOs.MembershipService.Requests;
using HostelFinder.Application.DTOs.MembershipService.Responses;
using HostelFinder.Application.Wrappers;
using HostelFinder.Application.DTOs.Image.Responses;
using HostelFinder.Application.DTOs.InVoice.Responses;
using HostelFinder.Application.DTOs.InVoice.Requests;
using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.DTOs.RoomTenancies.Request;
using HostelFinder.Application.DTOs.Vehicle.Request;
using HostelFinder.Application.DTOs.RentalContract.Request;
using HostelFinder.Application.DTOs.RentalContract.Response;
using HostelFinder.Application.DTOs.Invoice.Responses;
using HostelFinder.Application.DTOs.MaintenanceRecord.Request;
using HostelFinder.Application.DTOs.MaintenanceRecord.Response;
using HostelFinder.Application.DTOs.Vehicle.Responses;
using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.DTOs.AddressStory;
using HostelFinder.Application.DTOs.Room;
using HostelFinder.Application.DTOs.Story.Responses;
using HostelFinder.Application.DTOs.Notification;
using HostelFinder.Application.DTOs.Tenancies.Responses;
using HostelFinder.Application.Services;
using HostelFinder.Application.DTOs.MeterReading.Response;
using HostelFinder.Application.DTOs.MeterReading.Request;

namespace HostelFinder.Application.Mappings;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        // Post Mapping
        CreateMap<AddPostRequestDto, Post>().ReverseMap();

        CreateMap<Post, PostResponseDto>()
            .ForMember(dest => dest.MembershipServiceId, opt =>
                opt.MapFrom(src => src.MembershipServiceId))
            .ForMember(dest => dest.ImageUrls, opt =>
                opt.MapFrom(src => src.Images.Select(image => image.Url).ToList()))
            .ReverseMap();

        CreateMap<Post, ListPostsResponseDto>()
            .ForMember(dest => dest.Address, opt =>
                opt.MapFrom(src => src.Hostel.Address))
            .ForMember(dest => dest.MonthlyRentCost, opt =>
                opt.MapFrom(src => src.Room.MonthlyRentCost))
            .ForMember(dest => dest.Size, opt =>
                opt.MapFrom(src => src.Room.Size))
            .ForMember(dest => dest.FirstImage, opt =>
                opt.MapFrom(src => src.Images.Any()
                    ? src.Images.First().Url
                    : null))
            .ForMember(dest => dest.MembershipTag, opt =>
                opt.MapFrom(src => src.MembershipServices.Membership.Tag))
            .ReverseMap();

        CreateMap<UpdatePostRequestDto, Post>()
            .ForMember(dest => dest.HostelId, opt =>
                opt.MapFrom(src => src.HostelId))
            .ForMember(dest => dest.RoomId, opt =>
                opt.MapFrom(src => src.RoomId))
            .ForMember(dest => dest.Title, opt =>
                opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt =>
                opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt =>
                opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DateAvailable, opt =>
                opt.MapFrom(src => src.DateAvailable))
            .ReverseMap();

        // Hostel Mapping
        CreateMap<Hostel, HostelResponseDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.ImageUrl, opt =>
                opt.MapFrom(src => src.Images.FirstOrDefault().Url))
            .ForMember(dest => dest.Services, opt =>
                opt.MapFrom(src => src.HostelServices))
            .ReverseMap();

        CreateMap<Hostel, AddHostelRequestDto>()
            .ForMember(dest => dest.Address, opt =>
                opt.MapFrom(src => src.Address))
            .ReverseMap();

        CreateMap<Hostel, ListHostelResponseDto>()
            .ForMember(dest => dest.LandlordUserName, opt =>
                opt.MapFrom(src => src.Landlord.Username))
            .ForMember(dest => dest.ImageUrl, opt =>
                opt.MapFrom(src => src.Images.Any()
                    ? src.Images.First().Url
                    : null))
            .ReverseMap();

        // Address Mapping
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<AddressStory, AddressStoryDto>().ReverseMap();
        CreateMap<Hostel, UpdateHostelRequestDto>().ReverseMap();

        // RoomDetails Mapping
        CreateMap<RoomDetails, RoomDetailsResponseDto>().ReverseMap();
        CreateMap<RoomDetails, AddRoomDetailRequestDto>().ReverseMap();
        CreateMap<RoomDetails, UpdateRoomDetailsDto>().ReverseMap();

        // Amenities Mapping
        CreateMap<Amenity, AmenityResponse>().ReverseMap();
        CreateMap<RoomAmenities, AddRoomAmenityDto>().ReverseMap();
        CreateMap<AddAmenityDto, Amenity>().ReverseMap();

        // Service Cost Mapping
        CreateMap<ServiceCost, ServiceCostResponseDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
            .ForMember(dest => dest.HostelName, opt => opt.MapFrom(src => src.Hostel.HostelName))
            .ForMember(dest => dest.ChargingMethod, opt => opt.MapFrom(src => src.Service.ChargingMethod))
            .ReverseMap();
        CreateMap<ServiceCost, CreateServiceCostDto>().ReverseMap();
        CreateMap<UpdateServiceCostDto, ServiceCost>().ReverseMap();

        //Map User
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserProfileResponse, User>().ReverseMap();
        CreateMap<CreateUserRequestDto, User>().ReverseMap();

        CreateMap<UserProfileResponse, Response<UserProfileResponse>>()
            .ConstructUsing(src => new Response<UserProfileResponse>
            {
                Data = src,
                Succeeded = true
            });

        //Service Mapping
        CreateMap<ServiceCreateRequestDTO, Service>();
        CreateMap<ServiceUpdateRequestDTO, Service>();
        CreateMap<Service, ServiceResponseDTO>();

        // Membership mappings
        CreateMap<Membership, MembershipResponseDto>()
       .ForMember(dest => dest.MembershipServices, opt => opt.MapFrom(src => src.MembershipServices.FirstOrDefault()));

        CreateMap<AddMembershipRequestDto, Membership>().ReverseMap()
            .ForMember(dest => dest.MembershipServices, opt => opt.MapFrom(src => src.MembershipServices));
        CreateMap<UpdateMembershipRequestDto, Membership>().ReverseMap();
        CreateMap<AddUserMembershipRequestDto, UserMembership>()
              .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
              .ForMember(dest => dest.MembershipId, opt => opt.MapFrom(src => src.MembershipId));
        // Chuyển từ UserMembership sang UserMembershipStatistics
        CreateMap<UserMembership, UserMembershipStatistics>()
            .ForMember(dest => dest.MembershipDetails, opt => opt.MapFrom(src => src.Membership));
        // Chuyển từ UserMembership sang MembershipDetail
        CreateMap<UserMembership, MembershipDetail>()
            .ForMember(dest => dest.MembershipName, opt => opt.MapFrom(src => src.Membership.Name))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Membership.Price * (src.IsPaid ? 1 : 0)))
            .ForMember(dest => dest.TotalUsers, opt => opt.MapFrom(src => 1)); // Tổng số người dùng cho mỗi Membership
        // Chuyển từ Membership sang MembershipDetail (có thể dùng nếu cần)
        CreateMap<Membership, MembershipDetail>()
            .ForMember(dest => dest.MembershipName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.TotalUsers, opt => opt.Ignore()); // Không tính total users ở đây
        CreateMap<UpdateMembershipRequestDto, Membership>()
            .ForMember(dest => dest.MembershipServices, opt => opt.Ignore());

        // MembershipServices mappings
        CreateMap<MembershipServices, MembershipServiceResponseDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
            .ForMember(dest => dest.MaxPostsAllowed, opt => opt.MapFrom(src => src.MaxPostsAllowed))
            .ForMember(dest => dest.MaxPushTopAllowed, opt => opt.MapFrom(src => src.MaxPushTopAllowed))
            .ReverseMap();

        CreateMap<AddMembershipServiceReqDto, MembershipServices>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
            .ForMember(dest => dest.MaxPostsAllowed, opt => opt.MapFrom(src => src.MaxPostsAllowed))
            .ForMember(dest => dest.MaxPushTopAllowed, opt => opt.MapFrom(src => src.MaxPushTopAllowed))
            .ReverseMap();
        CreateMap<UpdateMembershipServiceReqDto, MembershipServices>()
           .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
           .ForMember(dest => dest.MaxPostsAllowed, opt => opt.MapFrom(src => src.MaxPostsAllowed))
           .ForMember(dest => dest.MaxPushTopAllowed, opt => opt.MapFrom(src => src.MaxPushTopAllowed))
           .ReverseMap();

        CreateMap<MembershipServiceResponseDto, AddMembershipServiceReqDto>().ReverseMap();

        CreateMap<MembershipServices, PostingMemberShipServiceDto>()
            .ForMember(dest => dest.TypeOfPost, opt
                => opt.MapFrom(src => src.Membership.Tag))
            .ForMember(dest => dest.NumberOfPostsRemaining, opt
                => opt.Ignore())
            .ForMember(dest => dest.NumberOfPushTopRemaining, opt
                => opt.Ignore());
        //Image Mapping
        CreateMap<Image, ImageResponseDto>().ReverseMap();

        //InVoice Mapping
        CreateMap<Invoice, InvoiceResponseDto>().ReverseMap();
        CreateMap<UpdateInvoiceRequestDto, Invoice>().ReverseMap();
        CreateMap<AddInVoiceRequestDto, Invoice>().ReverseMap();
        CreateMap<ListInvoiceResponseDto, Invoice>().ReverseMap()
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName));

        //Room Mapping
        CreateMap<Room, RoomResponseDto>().ReverseMap();
        CreateMap<UpdateRoomRequestDto, Room>();
        CreateMap<AddRoomRequestDto, Room>().ReverseMap();
        CreateMap<Room, RoomResponseDto>()
            .ForMember(dest => dest.HostelName, opt =>
                opt.MapFrom(src => src.Hostel.HostelName))
            /*.ForMember(dest => dest.ImageUrls, opt =>
                opt.MapFrom(src => src.Images.Select(image => image.Image).ToList()))*/
            .ReverseMap();
        CreateMap<AddRoomRequestDto, Room>()
            .ReverseMap();
        CreateMap<UpdateRoomRequestDto, Room>()
            .ReverseMap();
        CreateMap<Room, RoomByIdDto>()
            .ForMember(dest => dest.HostelName, opt =>
                opt.MapFrom(src => src.Hostel.HostelName))
            .ReverseMap();
        CreateMap<EditRoomDtoResponse, Room>()
            .ReverseMap();
        CreateMap<SelectRoomResponse, Room>().ReverseMap();


        //HostelService
        CreateMap<HostelServices, HostelServiceResponseDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Services.ServiceName))
            .ForMember(dest => dest.ChargingMethod, opt => opt.MapFrom(src => src.Services.ChargingMethod))
            .ForMember(dest => dest.HostelName, opt => opt.MapFrom(src => src.Hostel.HostelName))
            .ForMember(dest => dest.ServiceCost, opt => opt.MapFrom(src => src.Services.ServiceCosts))
            .ReverseMap();

        //Service Cost
        CreateMap<HostelServiceCostResponseDto, ServiceCost>().ReverseMap();

        //Terant Room
        CreateMap<AddRoomTenacyDto, RoomTenancy>()
            .ReverseMap();
        CreateMap<RoomTenancy, InformationTenacyReponseDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Tenant.FullName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Tenant.AvatarUrl))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Tenant.Phone))
            .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Tenant.Province))
            .ForMember(dest => dest.IdentityCardNumber, opt => opt.MapFrom(src => src.Tenant.IdentityCardNumber))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Tenant.Description))
            .ForMember(dest => dest.MoveInDate, opt => opt.MapFrom(src => src.MoveInDate))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Tenant.Email))
            .ReverseMap();
        CreateMap<RoomInfoDetailResponseDto, Room>().ReverseMap()
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Id));
        CreateMap<AddRoommateDto, Tenant>()
               .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
               .ForMember(dest => dest.FrontImageUrl, opt => opt.Ignore())
               .ForMember(dest => dest.BackImageUrl, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now));
        CreateMap<Tenant, TenantResponseDto>().ReverseMap();

        //Vehicle
        CreateMap<Vehicle, VehicleResponseDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ReverseMap();
        CreateMap<AddVehicleDto, Vehicle>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image != null ? "" : null))
            .ReverseMap();

        // Terance
        CreateMap<AddTenantDto, Tenant>().ReverseMap();
        CreateMap<TenantResponse, Tenant>().ReverseMap();

        // Rental Contract
        CreateMap<AddRentalContractDto, RentalContract>().ReverseMap();
        CreateMap<RoomContractHistoryResponseDto, RentalContract>().ReverseMap();
        CreateMap<RentalContract, RentalContractResponseDto>()
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.FullName))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName))
            .ReverseMap();

        //Invoice Detail
        CreateMap<InvoiceDetailResponseDto, InvoiceDetail>().ReverseMap();

        //MaintenanceRecord
        CreateMap<CreateMaintenanceRecordRequest, MaintenanceRecord>().ReverseMap();
        CreateMap<MaintenanceRecord, ListMaintenanceRecordResponseDto>()
            .ForMember(dest => dest.HostelName, opt => opt.MapFrom(src => src.Hostel.HostelName))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName))
            .ReverseMap();

        // Ánh xạ từ AddStoryRequestDto sang Story entity
        CreateMap<AddStoryRequestDto, Story>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
               .ForMember(dest => dest.AddressStory, opt => opt.Ignore());
        // Cấu hình ánh xạ từ Story entity sang StoryResponseDto
        CreateMap<Story, StoryResponseDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
            .ForMember(dest => dest.AddressStory, opt => opt.MapFrom(src => src.AddressStory));
        CreateMap<Story, ListStoryResponseDto>()
           .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).FirstOrDefault()))
           .ForMember(dest => dest.AddressStory, opt => opt.MapFrom(src => src.AddressStory));
        CreateMap<UpdateStoryRequestDto, Story>()
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())  
            .ForMember(dest => dest.Images, opt => opt.Ignore()) 
            .ForMember(dest => dest.AddressStory, opt => opt.MapFrom(src => src.AddressStory));
        //Map Notification
        CreateMap<Notification, NotificationResponseDto>()
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
        CreateMap<EditMaintenanceRecordDto, MaintenanceRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.MapFrom(src => DateTime.Now));

        CreateMap<MaintenanceRecord, MaintenanceRecordDetailsDto>()
          .ForMember(dest => dest.HostelName, opt => opt.MapFrom(src => src.Hostel.HostelName)) 
          .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName));

        CreateMap<MeterReading, MeterReadingDto>()
           .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName))
           .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName));

        CreateMap<EditMeterReadingDto, MeterReading>();
    }
}