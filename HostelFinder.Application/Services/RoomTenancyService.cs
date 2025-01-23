using AutoMapper;
using HostelFinder.Application.DTOs.RoomTenancies.Request;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class RoomTenancyService : IRoomTenancyService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTenancyRepository _roomTenancyRepository;
        private readonly IMapper _mapper;
        public RoomTenancyService(IRoomRepository roomRepository,
            IRoomTenancyRepository roomTenancyRepository,
            IMapper mapper)
        {
            _roomRepository = roomRepository;
            _roomTenancyRepository = roomTenancyRepository;
            _mapper = mapper;
        }

        public async Task<Response<string>> AddTenantToRoomAsync(AddRoomTenacyDto request)
        {
            // check user room exist
            var room = await _roomRepository.GetByIdAsync(request.RoomId);
            if (room == null)
            {
                return new Response<string> { Message = "Phòng trọ không tồn tại", Succeeded = false };
            }

            //check tenant now 
            var currentTenantsCount = await _roomTenancyRepository.CountCurrentTenantsAsync(request.RoomId);

            if (currentTenantsCount >= room.MaxRenters)
            {
                return new Response<string> { Message = "Phòng đã đạt số lượng người thuê tối đa.", Succeeded = false };
            }

            //Map to Domain 
            var roomTenacy = _mapper.Map<RoomTenancy>(request);
            roomTenacy.MoveInDate = DateTime.Now;
            roomTenacy.CreatedOn = DateTime.Now;

            //add to database
            await _roomTenancyRepository.AddAsync(roomTenacy);

            return new Response<string> { Message = $"Thêm người thuê ở phòng {room.RoomName} thành công ", Succeeded = true };

        }
    }
}
