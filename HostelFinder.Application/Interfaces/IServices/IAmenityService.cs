using HostelFinder.Application.DTOs.Amenity.Request;
using HostelFinder.Application.DTOs.Amenity.Response;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices;

public interface IAmenityService
{
    Task<Wrappers.Response<AmenityResponse>> AddAmenityAsync(AddAmenityDto addAmenityDto);
    Task<Wrappers.Response<bool>> DeleteAmenityAsync(Guid amenityId);
    Task<Wrappers.Response<List<AmenityResponse>>> GetAllAmenitiesAsync();
    Task<Response<IEnumerable<AmenityResponse>>> GetAmenitiesByRoomlIdAsync(Guid roomId);
}