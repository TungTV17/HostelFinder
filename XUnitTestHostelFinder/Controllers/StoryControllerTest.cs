
using HostelFinder.Application.DTOs.AddressStory;
using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.DTOs.Story.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class StoryControllerTest
    {
        private readonly StoryController _controller;
        private readonly Mock<IStoryService> _storyServiceMock;

        public StoryControllerTest()
        {
            _storyServiceMock = new Mock<IStoryService>();
            _controller = new StoryController(_storyServiceMock.Object);
        }

        [Fact]
        public async Task AddStory_ShouldReturnOk_WhenStoryIsAddedSuccessfully()
        {
            // Arrange
            var request = new AddStoryRequestDto
            {
                UserId = Guid.NewGuid(),
                Title = "Nice Story",
                MonthlyRentCost = 500,
                Description = "A lovely house.",
                Size = 50,
                RoomType = RoomType.Chung_cư_mini,
                DateAvailable = DateTime.Now.AddDays(10),
                AddressStory = new AddressStoryDto
                {
                    Commune = "123 Main St",
                    Province = "Hanoi",
                    District = "District 1",
                    DetailAddress = "100000"
                },
                Images = new List<IFormFile>() // Assuming some mock images are provided
            };

            _storyServiceMock.Setup(service => service.AddStoryAsync(It.IsAny<AddStoryRequestDto>()))
                             .ReturnsAsync(new Response<string>
                             {
                                 Succeeded = true,
                                 Message = "Thêm bài viết thành công",
                                 Data = Guid.NewGuid().ToString()
                             });

            // Act
            var result = await _controller.AddStory(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<string>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Thêm bài viết thành công", response.Message);
        }

        [Fact]
        public async Task AddStory_ShouldReturnBadRequest_WhenUserHasPosted5StoriesToday()
        {
            // Arrange
            var request = new AddStoryRequestDto
            {
                UserId = Guid.NewGuid(),
                Title = "Nice Story",
                MonthlyRentCost = 500,
                Description = "A lovely house.",
                Size = 50,
                RoomType = RoomType.Phòng_trọ,
                DateAvailable = DateTime.Now.AddDays(10),
                AddressStory = new AddressStoryDto
                {
                    Commune = "123 Main St",
                    Province = "Hanoi",
                    District = "District 1",
                    DetailAddress = "100000"
                },
                Images = new List<IFormFile>()
            };

            _storyServiceMock.Setup(service => service.AddStoryAsync(It.IsAny<AddStoryRequestDto>()))
                             .ReturnsAsync(new Response<string>
                             {
                                 Succeeded = false,
                                 Message = "Bạn đã đăng đủ 5 bài hôm nay. Vui lòng thử lại vào ngày mai."
                             });

            // Act
            var result = await _controller.AddStory(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Bạn đã đăng đủ 5 bài hôm nay. Vui lòng thử lại vào ngày mai.", response.Message);
        }

        [Fact]
        public async Task GetStoryById_ShouldReturnOk_WhenStoryExists()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var storyResponseDto = new StoryResponseDto
            {
                Title = "Nice Story",
                MonthlyRentCost = 500,
                Description = "A lovely house.",
                Size = 50,
                RoomType = RoomType.Chung_cư,
                DateAvailable = DateTime.Now.AddDays(10)
            };

            var response = new Response<StoryResponseDto>
            {
                Succeeded = true,
                Message = "Lấy bài viết thành công.",
                Data = storyResponseDto
            };

            // Mock the service method
            _storyServiceMock.Setup(service => service.GetStoryByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.GetStoryById(storyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(okResult.Value);
            Assert.True(responseValue.Succeeded);
            Assert.Equal("Lấy bài viết thành công.", responseValue.Message);
            Assert.NotNull(responseValue.Data);
        }

        [Fact]
        public async Task GetStoryById_ShouldReturnNotFound_WhenStoryDoesNotExist()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            var response = new Response<StoryResponseDto>
            {
                Succeeded = false,
                Message = "Bài đăng không tìm thấy."  // Match the controller's actual message
            };

            // Mock the service method
            _storyServiceMock.Setup(service => service.GetStoryByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.GetStoryById(storyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(notFoundResult.Value);
            Assert.False(responseValue.Succeeded);
            Assert.Equal("Bài đăng không tìm thấy.", responseValue.Message);  // Update to the correct message
        }

        [Fact]
        public async Task GetStoryById_ShouldReturnOk_WhenStoryFieldsAreValid()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var storyDto = new StoryResponseDto
            {
                Title = "Great House",
                Description = "A spacious, beautiful house.",
                MonthlyRentCost = 800,
                Size = 60,
                RoomType = RoomType.Chung_cư_mini,
                DateAvailable = DateTime.Now.AddDays(15),
                AddressStory = new AddressStoryDto
                {
                    Commune = "456 Another St",
                    Province = "Hanoi",
                    District = "District 2",
                    DetailAddress = "200000"
                }
            };

            var response = new Response<StoryResponseDto>
            {
                Succeeded = true,
                Message = "Lấy bài viết thành công.",
                Data = storyDto
            };

            // Mock the service method
            _storyServiceMock.Setup(service => service.GetStoryByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.GetStoryById(storyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(okResult.Value);
            Assert.True(responseValue.Succeeded);
            Assert.Equal("Lấy bài viết thành công.", responseValue.Message);
            Assert.Equal(storyDto.Title, responseValue.Data.Title);
            Assert.Equal(storyDto.Description, responseValue.Data.Description);
        }

        [Fact]
        public async Task GetStoryById_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Simulate an exception in the service
            _storyServiceMock.Setup(service => service.GetStoryByIdAsync(It.IsAny<Guid>()))
                             .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetStoryById(storyId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result); // Changed from StatusCodeResult to ObjectResult
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var errorResponse = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(errorResponse.Succeeded);
            Assert.Equal("Lỗi: Database error", errorResponse.Message);
        }

        [Fact]
        public async Task GetStoryById_ShouldReturnNotFound_WhenInvalidIdIsProvided()
        {
            // Arrange
            var invalidStoryId = Guid.NewGuid(); // Use a non-existing or invalid GUID

            var response = new Response<StoryResponseDto>
            {
                Succeeded = false,
                Message = "Bài đăng không tìm thấy." // Update expected message here
            };

            // Mock the service method to simulate no story found
            _storyServiceMock.Setup(service => service.GetStoryByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.GetStoryById(invalidStoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); // Assert that it's NotFound
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(notFoundResult.Value);
            Assert.False(responseValue.Succeeded);
            Assert.Equal("Bài đăng không tìm thấy.", responseValue.Message); // Update this line
        }

        [Fact]
        public async Task GetAllStoriesAsync_ShouldReturnOk_WhenStoriesAreFetchedSuccessfully()
        {
            // Arrange
            var filter = new StoryFilterDto { /* populate the filter fields if necessary */ };
            var pageIndex = 1;
            var pageSize = 10;

            var pagedResponse = new PagedResponse<List<ListStoryResponseDto>>
            {
                Succeeded = true,
                Message = "Stories fetched successfully.",
                Data = new List<ListStoryResponseDto> { new ListStoryResponseDto { /* populate fields */ } },
                TotalRecords = 1
            };

            // Mock the service method to return successful response
            _storyServiceMock.Setup(service => service.GetAllStoriesAsync(pageIndex, pageSize, filter))
                             .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAllStoriesAsync(filter, pageIndex, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListStoryResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Stories fetched successfully.", response.Message);
        }

        // Test 2: Should return BadRequest when service returns failure
        [Fact]
        public async Task GetAllStoriesAsync_ShouldReturnBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var filter = new StoryFilterDto { /* populate the filter fields if necessary */ };
            var pageIndex = 1;
            var pageSize = 10;

            var pagedResponse = new PagedResponse<List<ListStoryResponseDto>>
            {
                Succeeded = false,
                Message = "Failed to fetch stories.",
                Data = null,
                TotalRecords = 0
            };

            // Mock the service method to return failure response
            _storyServiceMock.Setup(service => service.GetAllStoriesAsync(pageIndex, pageSize, filter))
                             .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAllStoriesAsync(filter, pageIndex, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListStoryResponseDto>>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Failed to fetch stories.", response.Message);
        }

        // Test 3: Should return InternalServerError when an exception occurs
        [Fact]
        public async Task GetAllStoriesAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var filter = new StoryFilterDto { /* populate the filter fields if necessary */ };
            var pageIndex = 1;
            var pageSize = 10;

            // Simulate an exception occurring in the service
            _storyServiceMock.Setup(service => service.GetAllStoriesAsync(pageIndex, pageSize, filter))
                             .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllStoriesAsync(filter, pageIndex, pageSize);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Check if it's Internal Server Error (500)
            var response = Assert.IsType<Response<PagedResponse<ListStoryResponseDto>>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Lỗi server: Database error", response.Message);
        }


        // Test 1: Should return Ok when stories are fetched successfully
        [Fact]
        public async Task GetAllStoryForAdmin_ShouldReturnOk_WhenStoriesAreFetchedSuccessfully()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;

            var pagedResponse = new PagedResponse<List<ListStoryResponseDto>>
            {
                Succeeded = true,
                Message = "Stories fetched successfully.",
                Data = new List<ListStoryResponseDto> { new ListStoryResponseDto { /* populate fields */ } },
                TotalRecords = 1
            };

            // Mock the service method to return a successful paged response
            _storyServiceMock.Setup(service => service.GetAllStoryForAdminAsync(pageIndex, pageSize))
                             .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAllStoryForAdmin(pageIndex, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListStoryResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Stories fetched successfully.", response.Message);
        }

        // Test 2: Should return BadRequest when service returns failure
        [Fact]
        public async Task GetAllStoryForAdmin_ShouldReturnBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;

            var pagedResponse = new PagedResponse<List<ListStoryResponseDto>>
            {
                Succeeded = false,
                Message = "Failed to fetch stories.",
                Data = null,
                TotalRecords = 0
            };

            // Mock the service method to return a failed paged response
            _storyServiceMock.Setup(service => service.GetAllStoryForAdminAsync(pageIndex, pageSize))
                             .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAllStoryForAdmin(pageIndex, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListStoryResponseDto>>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Failed to fetch stories.", response.Message);
        }

        // Test 3: Should return InternalServerError when an exception occurs
        [Fact]
        public async Task GetAllStoryForAdmin_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;

            // Simulate an exception occurring in the service
            _storyServiceMock.Setup(service => service.GetAllStoryForAdminAsync(pageIndex, pageSize))
                             .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllStoryForAdmin(pageIndex, pageSize);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Verify it's a 500 Internal Server Error
            var response = Assert.IsType<Response<PagedResponse<ListStoryResponseDto>>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Lỗi server: Database error", response.Message);
        }

        // Test 1: Should return Ok when stories are fetched successfully
        [Fact]
        public async Task GetStoryByUserId_ShouldReturnOk_WhenStoriesAreFetchedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;

            var pagedResponse = new PagedResponse<List<ListStoryResponseDto>>
            {
                Succeeded = true,
                Message = "Stories fetched successfully.",
                Data = new List<ListStoryResponseDto>
            {
                new ListStoryResponseDto { /* populate fields */ }
            },
                TotalRecords = 1
            };

            // Mock the service method to return a successful paged response
            _storyServiceMock.Setup(service => service.GetStoryByUserIdAsync(userId, pageIndex, pageSize))
                             .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetStoryByUserId(userId, pageIndex, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListStoryResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Stories fetched successfully.", response.Message);
        }

        // Test 2: Should return BadRequest when no stories are found
        [Fact]
        public async Task GetStoryByUserId_ShouldReturnBadRequest_WhenNoStoriesFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;

            var pagedResponse = new PagedResponse<List<ListStoryResponseDto>>
            {
                Succeeded = false,
                Message = "Không có bài viết nào của người dùng này.",
                Data = null,
                TotalRecords = 0
            };

            // Mock the service method to return a failed paged response
            _storyServiceMock.Setup(service => service.GetStoryByUserIdAsync(userId, pageIndex, pageSize))
                             .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetStoryByUserId(userId, pageIndex, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListStoryResponseDto>>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Không có bài viết nào của người dùng này.", response.Message);
        }

        // Test 3: Should return InternalServerError when an exception occurs
        [Fact]
        public async Task GetStoryByUserId_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;

            // Simulate an exception occurring in the service
            _storyServiceMock.Setup(service => service.GetStoryByUserIdAsync(userId, pageIndex, pageSize))
                             .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetStoryByUserId(userId, pageIndex, pageSize);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Verify it's a 500 Internal Server Error
            var response = Assert.IsType<Response<PagedResponse<ListStoryResponseDto>>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Lỗi server: Database error", response.Message);
        }

        // Test 1: Should return Ok when the story is deleted successfully
        [Fact]
        public async Task DeleteStory_ShouldReturnOk_WhenStoryIsDeletedSuccessfully()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            var response = new Response<string>
            {
                Succeeded = true,
                Message = "Xóa bài viết thành công.",
                Data = storyId.ToString()
            };

            // Mock the service method to return a successful response
            _storyServiceMock.Setup(service => service.DeleteStoryAsync(storyId))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteStory(storyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<Response<string>>(okResult.Value);
            Assert.True(responseValue.Succeeded);
            Assert.Equal("Xóa bài viết thành công.", responseValue.Message);
        }

        // Test 2: Should return BadRequest when deletion fails (e.g., story not found)
        [Fact]
        public async Task DeleteStory_ShouldReturnBadRequest_WhenDeletionFails()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            var response = new Response<string>
            {
                Succeeded = false,
                Message = "Không thể tìm thấy bài viết.",
                Data = null
            };

            // Mock the service method to return a failed response
            _storyServiceMock.Setup(service => service.DeleteStoryAsync(storyId))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteStory(storyId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseValue = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(responseValue.Succeeded);
            Assert.Equal("Không thể tìm thấy bài viết.", responseValue.Message);
        }

        // Test 3: Should return InternalServerError when an exception occurs
        [Fact]
        public async Task DeleteStory_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Simulate an exception occurring in the service
            _storyServiceMock.Setup(service => service.DeleteStoryAsync(storyId))
                             .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteStory(storyId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Verify it's a 500 Internal Server Error
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Lỗi server: Database error", response.Message);
        }

        // Test 1: Should return BadRequest when the request data is invalid (null request)
        [Fact]
        public async Task UpdateStory_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            UpdateStoryRequestDto request = null;
            var images = new List<IFormFile>();
            var imageUrls = new List<string>();

            // Act
            var result = await _controller.UpdateStory(Guid.NewGuid(), request, images, imageUrls);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid input data.", response.Message);
        }

        // Test 2: Should return Ok when the story is updated successfully
        [Fact]
        public async Task UpdateStory_ShouldReturnOk_WhenStoryIsUpdatedSuccessfully()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var request = new UpdateStoryRequestDto
            {
                Title = "Updated Story",
                MonthlyRentCost = 500,
                Description = "Updated Description",
                Size = 100,
                RoomType = RoomType.Chung_cư,
                DateAvailable = DateTime.Now.AddMonths(1),
                AddressStory = new AddressStoryDto { /* mock data */ }
            };
            var images = new List<IFormFile>(); // No images for this test
            var imageUrls = new List<string>(); // No image URLs for this test

            var response = new Response<StoryResponseDto>
            {
                Succeeded = true,
                Message = "Cập nhật bài viết thành công.",
                Data = new StoryResponseDto { /* mock data */ }
            };

            _storyServiceMock.Setup(service => service.UpdateStoryAsync(storyId, request, images, imageUrls))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateStory(storyId, request, images, imageUrls);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(okResult.Value);
            Assert.True(responseValue.Succeeded);
            Assert.Equal("Cập nhật bài viết thành công.", responseValue.Message);
        }

        // Test 3: Should return BadRequest when the story is not found
        [Fact]
        public async Task UpdateStory_ShouldReturnBadRequest_WhenStoryNotFound()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var request = new UpdateStoryRequestDto
            {
                Title = "Updated Story",
                MonthlyRentCost = 500,
                Description = "Updated Description",
                Size = 100,
                RoomType = RoomType.Phòng_trọ,
                DateAvailable = DateTime.Now.AddMonths(1),
                AddressStory = new AddressStoryDto { /* mock data */ }
            };
            var images = new List<IFormFile>(); // No images for this test
            var imageUrls = new List<string>(); // No image URLs for this test

            var response = new Response<StoryResponseDto>("Story not found.");

            _storyServiceMock.Setup(service => service.UpdateStoryAsync(storyId, request, images, imageUrls))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateStory(storyId, request, images, imageUrls);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);
            Assert.False(responseValue.Succeeded);
            Assert.Equal("Story not found.", responseValue.Message);
        }

        // Test 4: Should return InternalServerError when an exception occurs during the update
        [Fact]
        public async Task UpdateStory_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var request = new UpdateStoryRequestDto
            {
                Title = "Updated Story",
                MonthlyRentCost = 500,
                Description = "Updated Description",
                Size = 100,
                RoomType = RoomType.Chung_cư_mini,
                DateAvailable = DateTime.Now.AddMonths(1),
                AddressStory = new AddressStoryDto { /* mock data */ }
            };
            var images = new List<IFormFile>(); // No images for this test
            var imageUrls = new List<string>(); // No image URLs for this test

            // Simulate an exception when updating the story
            _storyServiceMock.Setup(service => service.UpdateStoryAsync(storyId, request, images, imageUrls))
                             .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UpdateStory(storyId, request, images, imageUrls);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Verify it's a 500 Internal Server Error
            var response = Assert.IsType<Response<StoryResponseDto>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Lỗi server: Unexpected error", response.Message);
        }

        // Test 1: Should return BadRequest when the story is not found
        [Fact]
        public async Task DenyStory_ShouldReturnBadRequest_WhenStoryNotFound()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            var response = new Response<StoryResponseDto>("Story not found.");

            _storyServiceMock.Setup(service => service.DenyStoryAsync(storyId))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.DenyStory(storyId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);
            Assert.False(responseValue.Succeeded);
            Assert.Equal("Story not found.", responseValue.Message);
        }

        // Test 2: Should return Ok when the story is denied successfully
        [Fact]
        public async Task DenyStory_ShouldReturnOk_WhenStoryIsDeniedSuccessfully()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            var response = new Response<StoryResponseDto>
            {
                Succeeded = true,
                Message = "Bài đăng đã được từ chối.",
                Data = new StoryResponseDto { /* mock data */ }
            };

            _storyServiceMock.Setup(service => service.DenyStoryAsync(storyId))
                             .ReturnsAsync(response);

            // Act
            var result = await _controller.DenyStory(storyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<Response<StoryResponseDto>>(okResult.Value);
            Assert.True(responseValue.Succeeded);
            Assert.Equal("Bài đăng đã được từ chối.", responseValue.Message);
        }

        // Test 3: Should return InternalServerError when an exception occurs
        [Fact]
        public async Task DenyStory_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Simulate an exception when denying the story
            _storyServiceMock.Setup(service => service.DenyStoryAsync(storyId))
                             .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.DenyStory(storyId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Verify it's a 500 Internal Server Error
            var response = Assert.IsType<Response<StoryResponseDto>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Lỗi server: Unexpected error", response.Message);
        }

        [Fact]
        public async Task AcceptStory_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var errorMessage = "Database error occurred.";

            // Setup the mock service to throw an exception
            var storyServiceMock = new Mock<IStoryService>();
            storyServiceMock.Setup(service => service.AcceptStoryAsync(storyId))
                            .ThrowsAsync(new Exception(errorMessage));

            var controller = new StoryController(storyServiceMock.Object);

            // Act
            var result = await controller.AcceptStory(storyId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // Checking if it's ObjectResult
            Assert.Equal(500, statusCodeResult.StatusCode);  // Status code should be 500
            var response = Assert.IsType<Response<StoryResponseDto>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);  // The operation should not be successful
            Assert.Contains(errorMessage, response.Message);  // Message should contain the error message
        }

        [Fact]
        public async Task AcceptStory_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var storyId = Guid.NewGuid();
            var storyResponse = new Response<StoryResponseDto>
            {
                Succeeded = true,
                Message = "Bài đăng được chấp nhận.",
                Data = new StoryResponseDto()
            };

            // Mock service để trả về kết quả thành công
            var storyServiceMock = new Mock<IStoryService>();
            storyServiceMock.Setup(service => service.AcceptStoryAsync(storyId))
                            .ReturnsAsync(storyResponse);

            var controller = new StoryController(storyServiceMock.Object);

            // Act
            var result = await controller.AcceptStory(storyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Kiểm tra xem kết quả trả về là OkObjectResult
            var response = Assert.IsType<Response<StoryResponseDto>>(okResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded);  // Kiểm tra xem kết quả có thành công hay không
            Assert.Equal("Bài đăng được chấp nhận.", response.Message);  // Kiểm tra thông báo trả về
        }

        [Fact]
        public async Task AcceptStory_ShouldReturnBadRequest_WhenStoryAlreadyAccepted()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Giả lập một câu chuyện đã được chấp nhận
            var existingStory = new Story
            {
                Id = storyId,
                BookingStatus = BookingStatus.Accepted,
                CreatedOn = DateTime.Now
            };

            var storyServiceMock = new Mock<IStoryService>();
            storyServiceMock.Setup(service => service.AcceptStoryAsync(storyId))
                            .ReturnsAsync(new Response<StoryResponseDto>
                            {
                                Succeeded = false,
                                Message = "Story has already been accepted."
                            });

            var controller = new StoryController(storyServiceMock.Object);

            // Act
            var result = await controller.AcceptStory(storyId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra xem kết quả trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Equal("Story has already been accepted.", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task AcceptStory_ShouldReturnBadRequest_WhenServiceReturnsEmptyData()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Mock service trả về kết quả hợp lệ nhưng không có dữ liệu
            var storyServiceMock = new Mock<IStoryService>();
            storyServiceMock.Setup(service => service.AcceptStoryAsync(storyId))
                            .ReturnsAsync(new Response<StoryResponseDto>
                            {
                                Succeeded = false,
                                Message = "No content found for story."
                            });

            var controller = new StoryController(storyServiceMock.Object);

            // Act
            var result = await controller.AcceptStory(storyId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra xem kết quả trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Equal("No content found for story.", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task AcceptStory_ShouldReturnBadRequest_WhenInvalidData()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Mock service trả về lỗi khi có dữ liệu không hợp lệ
            var storyServiceMock = new Mock<IStoryService>();
            storyServiceMock.Setup(service => service.AcceptStoryAsync(storyId))
                            .ReturnsAsync(new Response<StoryResponseDto>("Invalid input data."));

            var controller = new StoryController(storyServiceMock.Object);

            // Act
            var result = await controller.AcceptStory(storyId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra xem kết quả trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Equal("Invalid input data.", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task AcceptStory_ShouldReturnBadRequest_WhenStoryNotFound()
        {
            // Arrange
            var storyId = Guid.NewGuid();

            // Mock service trả về lỗi khi không tìm thấy câu chuyện
            var storyServiceMock = new Mock<IStoryService>();
            storyServiceMock.Setup(service => service.AcceptStoryAsync(storyId))
                            .ReturnsAsync(new Response<StoryResponseDto>("Story not found."));

            var controller = new StoryController(storyServiceMock.Object);

            // Act
            var result = await controller.AcceptStory(storyId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra xem kết quả trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<StoryResponseDto>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Equal("Story not found.", response.Message);  // Kiểm tra thông báo lỗi đúng
        }
      

    }
}

