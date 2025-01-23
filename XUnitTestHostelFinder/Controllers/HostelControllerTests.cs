using HostelFinder.Application.DTOs.Address;
using HostelFinder.Application.DTOs.Hostel.Requests;
using HostelFinder.Application.DTOs.Hostel.Responses;
using HostelFinder.Application.DTOs.Image.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class HostelControllerTests
    {
        private readonly Mock<IHostelService> _hostelServiceMock;
        private readonly Mock<IS3Service> _s3ServiceMock;
        private readonly HostelController _controller;

        public HostelControllerTests()
        {
            _hostelServiceMock = new Mock<IHostelService>();
            _s3ServiceMock = new Mock<IS3Service>();
            _controller = new HostelController(_hostelServiceMock.Object, _s3ServiceMock.Object);
        }

        [Fact]
        public async Task GetHostelById_ReturnsNotFound_WhenHostelDoesNotExist()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var response = new Response<HostelResponseDto>("Hostel not found.")
            {
                Succeeded = false
            };

            _hostelServiceMock.Setup(service => service.GetHostelByIdAsync(hostelId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetHostelById(hostelId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<Response<HostelResponseDto>>(notFoundResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Hostel not found.", returnValue.Message);
        }

        [Fact]
        public async Task GetHostelById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            _hostelServiceMock.Setup(service => service.GetHostelByIdAsync(hostelId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetHostelById(hostelId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var returnValue = Assert.IsType<Response<HostelResponseDto>>(objectResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Internal server error", returnValue.Message);
        }


        [Fact]
        public async Task AddHostel_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("HostelName", "Required");

            // Act
            var result = await _controller.AddHostel(null, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }



        [Fact]
        public async Task DeleteHostel_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var response = new Response<bool> { Succeeded = true, Data = true, Message = "Delete successful." };

            _hostelServiceMock.Setup(s => s.DeleteHostelAsync(hostelId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteHostel(hostelId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.True(responseData.Data);
            Assert.Equal("Delete successful.", responseData.Message);
        }


        [Fact]
        public async Task DeleteHostel_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var hostelId = Guid.NewGuid();

            _hostelServiceMock.Setup(service => service.DeleteHostelAsync(hostelId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteHostel(hostelId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseData = Assert.IsType<Response<bool>>(objectResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Internal server error: Internal server error", responseData.Message);
        }


        [Fact]
        public async Task DeleteHostel_ReturnsNotFound_WhenFailed()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var response = new Response<bool> { Succeeded = false, Data = false, Message = "Hostel not found" };

            _hostelServiceMock.Setup(s => s.DeleteHostelAsync(hostelId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteHostel(hostelId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.False(responseData.Data);
            Assert.Equal("Hostel not found", responseData.Message);
        }


        [Fact]
        public async Task GetHostelsByLandlordId_ReturnsNotFound_WhenNoHostelsExist()
        {
            // Arrange
            var landlordId = Guid.NewGuid();
            var response = new PagedResponse<List<ListHostelResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "No hostels found" }
            };

            _hostelServiceMock.Setup(service => service.GetHostelsByUserIdAsync(landlordId, null, null, null, null, null))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetHostelsByLandlordId(landlordId, null, null, null, null, null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errors = Assert.IsType<List<string>>(notFoundResult.Value);
            Assert.Contains("No hostels found", errors);
        }

        [Fact]
        public async Task GetHostelsByLandlordId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var landlordId = Guid.NewGuid();
            var searchPhrase = "Hostel";

            _hostelServiceMock.Setup(service => service.GetHostelsByUserIdAsync(landlordId, searchPhrase, null, null, null, null))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetHostelsByLandlordId(landlordId, searchPhrase, null, null, null, null);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error", objectResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenHostelsExist()
        {
            // Arrange
            var query = new GetAllHostelQuery
            {
                SearchPhrase = "Hostel",
                PageSize = 2,
                PageNumber = 1,
                SortBy = "HostelName",
                SortDirection = SortDirection.Ascending
            };

            var hostels = new List<ListHostelResponseDto>
    {
        new ListHostelResponseDto { Id = Guid.NewGuid(), HostelName = "Hostel A" },
        new ListHostelResponseDto { Id = Guid.NewGuid(), HostelName = "Hostel B" }
    };

            var response = new PagedResponse<List<ListHostelResponseDto>>(hostels, query.PageNumber, query.PageSize)
            {
                TotalRecords = 5,
                TotalPages = 3,
            };

            _hostelServiceMock.Setup(service => service.GetAllHostelAsync(query))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<PagedResponse<List<ListHostelResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal(2, responseData.Data.Count);
            Assert.Equal(5, responseData.TotalRecords);
            Assert.Equal(3, responseData.TotalPages);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenNoHostelsMatch()
        {
            // Arrange
            var query = new GetAllHostelQuery
            {
                SearchPhrase = "NonExistentHostel",
                PageSize = 2,
                PageNumber = 1,
                SortBy = "HostelName",
                SortDirection = SortDirection.Descending
            };

            var response = new PagedResponse<List<ListHostelResponseDto>>(null, query.PageNumber, query.PageSize)
            {
                Succeeded = false,
                Message = "No hostels found."
            };

            _hostelServiceMock.Setup(service => service.GetAllHostelAsync(query))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<PagedResponse<List<ListHostelResponseDto>>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("No hostels found.", responseData.Message);
        }

        [Fact]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var query = new GetAllHostelQuery
            {
                SearchPhrase = "Hostel",
                PageSize = 2,
                PageNumber = 1,
                SortBy = "HostelName",
                SortDirection = SortDirection.Ascending
            };

            _hostelServiceMock.Setup(service => service.GetAllHostelAsync(query))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error", objectResult.Value);
        }

        [Theory]
        [InlineData(SortDirection.Ascending, "Ascending")]
        [InlineData(SortDirection.Descending, "Descending")]
        public async Task GetAll_HandlesSortDirectionParameter(SortDirection sortDirection, string sortDirectionText)
        {
            // Arrange
            var query = new GetAllHostelQuery
            {
                SearchPhrase = "Hostel",
                PageSize = 2,
                PageNumber = 1,
                SortBy = "HostelName",
                SortDirection = sortDirection
            };

            var hostels = new List<ListHostelResponseDto>
    {
        new ListHostelResponseDto { Id = Guid.NewGuid(), HostelName = $"Hostel {sortDirectionText}" }
    };

            // Use the three-argument constructor and then set additional properties
            var response = new PagedResponse<List<ListHostelResponseDto>>(hostels, query.PageNumber, query.PageSize)
            {
                TotalRecords = 1, // Total number of records
                TotalPages = 1 // Since there's only 1 record
            };

            _hostelServiceMock.Setup(service => service.GetAllHostelAsync(query))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<PagedResponse<List<ListHostelResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Single(responseData.Data);
            Assert.Equal($"Hostel {sortDirectionText}", responseData.Data.First().HostelName);
            Assert.Equal(1, responseData.TotalRecords);
            Assert.Equal(1, responseData.TotalPages);
        }


    }
}
