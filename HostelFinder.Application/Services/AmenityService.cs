using AutoMapper;
using HostelFinder.Application.DTOs.Amenity.Request;
using HostelFinder.Application.DTOs.Amenity.Response;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services;

public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;


    public AmenityService(IAmenityRepository amenityRepository, IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }

    public async Task<Response<AmenityResponse>> AddAmenityAsync(AddAmenityDto addAmenityDto)
    {
        if (await _amenityRepository.ExistsByNameAsync(addAmenityDto.AmenityName))
        {
            return new Response<AmenityResponse>("An amenity with the same name already exists.");
        }
        try
        {

        }
        catch (Exception ex)
        {
        }
        var amenity = _mapper.Map<Amenity>(addAmenityDto);
        var addedAmenity = await _amenityRepository.AddAsync(amenity);
        var response = _mapper.Map<AmenityResponse>(addedAmenity);
        return new Response<AmenityResponse>(response, "Amenity added successfully");
    }

    public async Task<Response<bool>> DeleteAmenityAsync(Guid amenityId)
    {
        var amenity = await _amenityRepository.GetByIdAsync(amenityId);
        if (amenity == null)
        {
            return new Response<bool>(false, "Amenity not found");
        }

        try
        {
            var deletedEntity = await _amenityRepository.DeletePermanentAsync(amenityId);
            if (deletedEntity == null)
            {
                return new Response<bool>(false, "Failed to delete amenity");
            }

            return new Response<bool>(true, "Amenity deleted successfully");
        }
        catch (Exception ex)
        {
            return new Response<bool>(false, message: ex.Message);
        }
    }


    public async Task<Response<List<AmenityResponse>>> GetAllAmenitiesAsync()
    {
        var amenities = await _amenityRepository.ListAllAsync();
        var amenityResponses = _mapper.Map<List<AmenityResponse>>(amenities);
        return new Response<List<AmenityResponse>>(amenityResponses, "Danh sách tiện ích");
    }

    public async Task<Response<IEnumerable<AmenityResponse>>> GetAmenitiesByRoomlIdAsync(Guid roomId)
    {
        var amenities = await _amenityRepository.GetAmenitysByRoomIdAsync(roomId);
        var amenityResponses = _mapper.Map<IEnumerable<AmenityResponse>>(amenities);
        return new Response<IEnumerable<AmenityResponse>>(amenityResponses, "Danh sách tiện ích");
    }
}