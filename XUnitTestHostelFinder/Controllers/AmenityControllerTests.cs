using HostelFinder.Application.DTOs.Amenity.Request;
using HostelFinder.Application.DTOs.Amenity.Response;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class AmenityControllerTests
    {
        private readonly Mock<IAmenityService> _amenityServiceMock;
        private readonly AmenityController _controller;

        public AmenityControllerTests()
        {
            _amenityServiceMock = new Mock<IAmenityService>();
            _controller = new AmenityController(_amenityServiceMock.Object);
        }

        // 1. Test AddAmenity with valid data
        [Fact]
        public async Task AddAmenity_ReturnsOkResult_WhenDataIsValid()
        {
            // Arrange
            var addAmenityDto = new AddAmenityDto { AmenityName = "WiFi" };
            var mockResponse = new Response<AmenityResponse>(new AmenityResponse { AmenityName = "WiFi" }, "Amenity added successfully");

            _amenityServiceMock
                .Setup(service => service.AddAmenityAsync(addAmenityDto))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddAmenity(addAmenityDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<AmenityResponse>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("Amenity added successfully", returnValue.Message);
        }

        // 2. Test AddAmenity with invalid model state
        [Fact]
        public async Task AddAmenity_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("AmenityName", "Required");

            // Act
            var result = await _controller.AddAmenity(new AddAmenityDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<AmenityResponse>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid model state", response.Message);
        }


        // 3. Test AddAmenity with server exception
        [Fact]
        public async Task AddAmenity_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var addAmenityDto = new AddAmenityDto { AmenityName = "Pool" };
            _amenityServiceMock
                .Setup(service => service.AddAmenityAsync(addAmenityDto))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.AddAmenity(addAmenityDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error", objectResult.Value);
        }

        // 4. Test GetAllAmenities with amenities present
        [Fact]
        public async Task GetAllAmenities_ReturnsOkResult_WhenAmenitiesExist()
        {
            // Arrange
            var amenities = new List<AmenityResponse>
            {
                new AmenityResponse { AmenityName = "WiFi" },
                new AmenityResponse { AmenityName = "Parking" }
            };
            var mockResponse = new Response<List<AmenityResponse>>(amenities, "Amenities retrieved successfully");

            _amenityServiceMock
                .Setup(service => service.GetAllAmenitiesAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllAmenities();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<List<AmenityResponse>>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal(amenities.Count, returnValue.Data.Count);
        }

        // 5. Test GetAllAmenities when no amenities found
        [Fact]
        public async Task GetAllAmenities_ReturnsNotFound_WhenNoAmenitiesExist()
        {
            // Arrange
            var mockResponse = new Response<List<AmenityResponse>>(null, "No amenities found")
            {
                Succeeded = false,
                Errors = new List<string> { "No amenities found" }
            };

            _amenityServiceMock
                .Setup(service => service.GetAllAmenitiesAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllAmenities();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<Response<List<AmenityResponse>>>(notFoundResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("No amenities found", returnValue.Message);
        }


        // 6. Test DeleteAmenity with successful deletion
        [Fact]
        public async Task DeleteAmenity_ReturnsOkResult_WhenAmenityIsDeleted()
        {
            // Arrange
            var amenityId = Guid.NewGuid();
            var response = new Response<bool>(true, "Amenity deleted successfully");

            _amenityServiceMock.Setup(service => service.DeleteAmenityAsync(amenityId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteAmenity(amenityId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal("Amenity deleted successfully", responseData.Message);
        }

        [Fact]
        public async Task DeleteAmenity_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var amenityId = Guid.NewGuid();
            _amenityServiceMock.Setup(service => service.DeleteAmenityAsync(amenityId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteAmenity(amenityId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error: Internal server error", objectResult.Value);
        }

        // 8. Parameterized Test for AddAmenity with valid and invalid data
        [Theory]
        [InlineData("WiFi", true)]
        [InlineData("", false)] // Invalid: Empty AmenityName
        public async Task AddAmenity_WithDifferentData_ReturnsExpectedResult(string amenityName, bool shouldSucceed)
        {
            // Arrange
            var addAmenityDto = new AddAmenityDto { AmenityName = amenityName };
            var mockResponse = new Response<AmenityResponse>(
                shouldSucceed ? new AmenityResponse { AmenityName = amenityName } : null,
                shouldSucceed ? "Amenity added successfully" : "Invalid amenity data"
            )
            {
                Succeeded = shouldSucceed,
                Errors = !shouldSucceed ? new List<string> { "Invalid amenity data" } : null
            };

            _amenityServiceMock
                .Setup(service => service.AddAmenityAsync(addAmenityDto))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddAmenity(addAmenityDto);

            // Assert
            if (shouldSucceed)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsType<Response<AmenityResponse>>(okResult.Value);
                Assert.Equal("Amenity added successfully", returnValue.Message);
            }
            else
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var returnValue = Assert.IsType<Response<AmenityResponse>>(badRequestResult.Value);
                Assert.Contains("Invalid amenity data", returnValue.Errors);
            }
        }

        [Fact]
        public async Task GetAllAmenities_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _amenityServiceMock
                .Setup(service => service.GetAllAmenitiesAsync())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetAllAmenities();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error", objectResult.Value);
        }

        [Fact]
        public async Task AddAmenity_ReturnsBadRequest_WhenAmenityNameIsNull()
        {
            // Arrange
            var addAmenityDto = new AddAmenityDto { AmenityName = null };
            _controller.ModelState.AddModelError("AmenityName", "Required");

            // Act
            var result = await _controller.AddAmenity(addAmenityDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<AmenityResponse>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid model state", response.Message);
        }

        [Fact]
        public async Task GetAmenitiesByRoomId_ReturnsOkResult_WhenAmenitiesExist()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var mockAmenities = new List<AmenityResponse>
    {
        new AmenityResponse { Id = Guid.NewGuid(), AmenityName = "WiFi" },
        new AmenityResponse { Id = Guid.NewGuid(), AmenityName = "Parking" }
    };

            var response = new Response<IEnumerable<AmenityResponse>>(mockAmenities, "Danh sách tiện ích");

            _amenityServiceMock.Setup(service => service.GetAmenitiesByRoomlIdAsync(roomId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAmenitiesByRoomId(roomId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<IEnumerable<AmenityResponse>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal(2, responseData.Data.Count());
            Assert.Equal("Danh sách tiện ích", responseData.Message);
        }

        [Fact]
        public async Task GetAmenitiesByRoomId_ReturnsNotFound_WhenNoAmenitiesExist()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var response = new Response<IEnumerable<AmenityResponse>>(null, "No amenities found")
            {
                Succeeded = false
            };

            _amenityServiceMock.Setup(service => service.GetAmenitiesByRoomlIdAsync(roomId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAmenitiesByRoomId(roomId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<IEnumerable<AmenityResponse>>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("No amenities found", responseData.Message);
        }

        [Fact]
        public async Task GetAmenitiesByRoomId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var roomId = Guid.NewGuid();

            _amenityServiceMock.Setup(service => service.GetAmenitiesByRoomlIdAsync(roomId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetAmenitiesByRoomId(roomId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<IEnumerable<AmenityResponse>>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Internal server error", response.Message);
        }



    }
}
