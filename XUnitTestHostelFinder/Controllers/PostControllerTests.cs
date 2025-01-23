using HostelFinder.Application.DTOs.Address;
using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Services;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class PostControllerTests
    {
        private readonly PostController _controller;
        private readonly Mock<IPostService> _postServiceMock;
        private readonly Mock<IS3Service> _s3ServiceMock;
        private readonly Mock<IOpenAiService> _openAiServiceMock;

        public PostControllerTests()
        {
            _postServiceMock = new Mock<IPostService>();
            _s3ServiceMock = new Mock<IS3Service>();
            _openAiServiceMock = new Mock<IOpenAiService>();

            _controller = new PostController(_postServiceMock.Object, _s3ServiceMock.Object, _openAiServiceMock.Object);
        }


        [Fact]
        public async Task GetAllPostWithPriceAndStatusAndTime_ShouldReturnOk_WhenPostsExist()
        {
            // Arrange
            var mockResponse = new Response<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = new List<ListPostsResponseDto>
            {
                new ListPostsResponseDto { /* populate fields as needed */ },
                new ListPostsResponseDto { /* populate fields as needed */ }
            }
            };

            _postServiceMock.Setup(service => service.GetAllPostWithPriceAndStatusAndTime())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllPostWithPriceAndStatusAndTime();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<ListPostsResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotNull(response.Data);
            Assert.True(response.Data.Any());
        }

        [Fact]
        public async Task GetAllPostWithPriceAndStatusAndTime_ShouldReturnNotFound_WhenNoPostsExist()
        {
            // Arrange
            var mockResponse = new Response<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = new List<ListPostsResponseDto>() // Empty list
            };

            _postServiceMock.Setup(service => service.GetAllPostWithPriceAndStatusAndTime())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllPostWithPriceAndStatusAndTime();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<List<ListPostsResponseDto>>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Empty(response.Data);
            Assert.Equal("No posts found", response.Message);
        }

        [Fact]
        public async Task GetAllPostWithPriceAndStatusAndTime_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _postServiceMock.Setup(service => service.GetAllPostWithPriceAndStatusAndTime())
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetAllPostWithPriceAndStatusAndTime();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Something went wrong", response.Message);
        }

        [Fact]
        public async Task GetAllPostWithPriceAndStatusAndTimePaging_ShouldReturnOk_WhenPostsExist()
        {
            // Arrange: Create a mock PagedResponse for posts with necessary fields
            var mockPagedResponse = new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = new List<ListPostsResponseDto>
        {
            new ListPostsResponseDto
            {
                Id = Guid.NewGuid(),
                Title = "Post 1",
                Description = "Description of Post 1",
                Address = new AddressDto
                {
                    Commune = "123 Main St",
                    Province = "Cityville",
                    District = "State",
                    DetailAddress = "12345"
                },
                MonthlyRentCost = 1000,
                Size = 25.5m,
                MembershipTag = "Premium",
                FirstImage = "image1.jpg",
                CreatedOn = DateTimeOffset.Now,
                Status = true
            },
            new ListPostsResponseDto
            {
                Id = Guid.NewGuid(),
                Title = "Post 2",
                Description = "Description of Post 2",
                Address = new AddressDto
                {
                    Commune = "456 Oak St",
                    Province = "Townsville",
                    District = "State",
                    DetailAddress = "67890"
                },
                MonthlyRentCost = 1200,
                Size = 30.5m,
                MembershipTag = "Standard",
                FirstImage = "image2.jpg",
                CreatedOn = DateTimeOffset.Now,
                Status = true
            }
        },
                TotalRecords = 2
            };

            // Mock the service call to return a PagedResponse
            _postServiceMock.Setup(service => service.GetAllPostWithPriceAndStatusAndTime(1, 10))
                .ReturnsAsync(mockPagedResponse);

            // Act: Call the controller action
            var result = await _controller.GetAllPostWithPriceAndStatusAndTimePaging(1, 10);

            // Assert: Verify the response is OK and contains the expected data
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListPostsResponseDto>>>(okResult.Value);

            // Check the success flag
            Assert.True(response.Succeeded);

            // Ensure we have posts returned
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count);  // Ensure we have two posts

            // Check if TotalRecords is set correctly
            Assert.Equal(2, response.TotalRecords);

            // Check the properties of the first post to ensure everything is mapped correctly
            var firstPost = response.Data[0];
            Assert.Equal("Post 1", firstPost.Title);
            Assert.Equal("Description of Post 1", firstPost.Description);
            Assert.Equal("123 Main St", firstPost.Address.Commune);
            Assert.Equal("Cityville", firstPost.Address.Province);
            Assert.Equal("State", firstPost.Address.District);
            Assert.Equal("12345", firstPost.Address.DetailAddress);
            Assert.Equal(1000, firstPost.MonthlyRentCost);
            Assert.Equal(25.5m, firstPost.Size);
            Assert.Equal("Premium", firstPost.MembershipTag);
            Assert.Equal("image1.jpg", firstPost.FirstImage);
            Assert.True(firstPost.Status);
        }

        [Fact]
        public async Task GetAllPostWithPriceAndStatusAndTimePaging_ShouldReturnNotFound_WhenNoPostsExist()
        {
            // Arrange: Create a mock PagedResponse with no posts
            var mockPagedResponse = new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = new List<ListPostsResponseDto>(),  // No posts found
                TotalRecords = 0
            };

            // Mock the service call to return the empty PagedResponse
            _postServiceMock.Setup(service => service.GetAllPostWithPriceAndStatusAndTime(1, 10))
                .ReturnsAsync(mockPagedResponse);

            // Act: Call the controller action
            var result = await _controller.GetAllPostWithPriceAndStatusAndTimePaging(1, 10);

            // Assert: Verify the response is NotFound
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<List<ListPostsResponseDto>>>(notFoundResult.Value);

            // Check the success flag is false
            Assert.False(response.Succeeded);

            // Check that the message is "No posts found"
            Assert.Equal("No posts found", response.Message);

            // Ensure the Data is an empty list
            Assert.Empty(response.Data);
        }

        [Fact]
        public async Task GetAllPostWithPriceAndStatusAndTimePaging_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange: Setup the mock to throw an exception when the service method is called
            _postServiceMock.Setup(service => service.GetAllPostWithPriceAndStatusAndTime(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("An unexpected error occurred"));

            // Act: Call the controller action
            var result = await _controller.GetAllPostWithPriceAndStatusAndTimePaging(1, 10);

            // Assert: Verify the response is InternalServerError
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);

            var response = Assert.IsType<Response<string>>(internalServerErrorResult.Value);

            // Check that the response is not succeeded and contains the error message
            Assert.False(response.Succeeded);
            Assert.Contains("Internal server error", response.Message);
        }

        //[Fact]
        //public async Task AddPostAsync_ShouldReturnFailed_WhenMembershipServiceIsNotInitialized()
        //{
        //    // Arrange: Set _membershipService to null
        //    _membershipServiceMock = null;

        //    var postDto = new AddPostRequestDto
        //    {
        //        HostelId = Guid.NewGuid(),
        //        RoomId = Guid.NewGuid(),
        //        Title = "Test Title",
        //        Description = "Test Description",
        //        DateAvailable = DateTime.Now
        //    };

        //    var imageUrls = new List<string> { "https://example.com/image1.jpg" };
        //    var userId = Guid.NewGuid();

        //    // Act: Call AddPostAsync
        //    var result = await _postService.AddPostAsync(postDto, imageUrls, userId);

        //    // Assert: Ensure that the response indicates failure due to uninitialized membership service
        //    Assert.False(result.Succeeded);
        //    Assert.Equal("Membership service not initialized.", result.Message);
        //}

        [Fact]
        public async Task AddPostAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var postDto = new AddPostRequestDto
            {
                HostelId = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                Title = "Test Title",
                Description = "Test Description",
                DateAvailable = DateTime.Now
            };

            var userId = Guid.NewGuid();

            _postServiceMock.Setup(service => service.AddPostAsync(It.IsAny<AddPostRequestDto>(), It.IsAny<List<string>>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.AddPost(userId, postDto, new List<IFormFile>());

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            var response = Assert.IsType<Response<string>>(internalServerErrorResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Internal server error", response.Message);
        }

        [Fact]
        public async Task UpdatePostAsync_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var postDto = new UpdatePostRequestDto();  // Invalid data
            _controller.ModelState.AddModelError("Title", "Title is required"); // Force model state invalid

            // Act
            var result = await _controller.UpdatePost(postId, postDto, null, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task DeletePost_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _postServiceMock.Setup(service => service.DeletePostAsync(postId, userId))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Internal server error", response.Message);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenPostsAreFound()
        {
            // Arrange
            var request = new GetAllPostsQuery
            {
                SearchPhrase = "Test",
                PageSize = 10,
                PageNumber = 1,
                SortBy = "Date",
                SortDirection = SortDirection.Ascending
            };

            var mockPostsResponse = new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = new List<ListPostsResponseDto> { new ListPostsResponseDto { Id = Guid.NewGuid(), Title = "Test Post" } },
                TotalRecords = 1,
                Message = "Posts retrieved successfully."
            };

            _postServiceMock.Setup(service => service.GetAllPostAysnc(request))
                .ReturnsAsync(mockPostsResponse);

            // Act
            var result = await _controller.Get(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<List<ListPostsResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Posts retrieved successfully.", response.Message);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var request = new GetAllPostsQuery
            {
                SearchPhrase = "Invalid",
                PageSize = 10,
                PageNumber = 1,
                SortBy = "Date",
                SortDirection = SortDirection.Ascending
            };

            var mockFailedResponse = new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "No posts found." }
            };

            _postServiceMock.Setup(service => service.GetAllPostAysnc(request))
                .ReturnsAsync(mockFailedResponse);

            // Act
            var result = await _controller.Get(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("No posts found.", response.Errors);
        }

        [Fact]
        public async Task Get_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new GetAllPostsQuery
            {
                SearchPhrase = "Test",
                PageSize = 10,
                PageNumber = 1,
                SortBy = "Date",
                SortDirection = SortDirection.Ascending
            };

            _postServiceMock.Setup(service => service.GetAllPostAysnc(request))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Get(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task GetPostsByUserId_ShouldReturnBadRequest_WhenUserIdIsInvalid()
        {
            // Arrange
            var invalidUserId = Guid.Empty;

            // Act
            var result = await _controller.GetPostsByUserId(invalidUserId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid user ID.", response.Message);
        }

        [Fact]
        public async Task GetPostsByUserId_ShouldReturnNotFound_WhenUserHasNoPosts()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Mock the service to return no posts
            var emptyResponse = new Response<List<ListPostsResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "Bạn chưa có bài đăng nào." }
            };

            _postServiceMock.Setup(service => service.GetPostsByUserIdAsync(userId))
                .ReturnsAsync(emptyResponse);

            // Act
            var result = await _controller.GetPostsByUserId(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Bạn chưa có bài đăng nào.", response.Errors.First());
        }

        [Fact]
        public async Task GetPostsByUserId_ShouldReturnOk_WhenUserHasPosts()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Mock the service to return some posts
            var mockPosts = new List<ListPostsResponseDto>
        {
            new ListPostsResponseDto { Id = Guid.NewGuid(), Title = "Test Post 1" },
            new ListPostsResponseDto { Id = Guid.NewGuid(), Title = "Test Post 2" }
        };

            var successResponse = new Response<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = mockPosts
            };

            _postServiceMock.Setup(service => service.GetPostsByUserIdAsync(userId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.GetPostsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<ListPostsResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(2, response.Data.Count);
            Assert.Equal("Test Post 1", response.Data.First().Title);
        }

        [Fact]
        public async Task GetPostById_ShouldReturnBadRequest_WhenPostIdIsInvalid()
        {
            // Arrange
            var invalidPostId = Guid.Empty;

            // Act
            var result = await _controller.GetPostById(invalidPostId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid post ID.", response.Message);
        }

        [Fact]
        public async Task GetPostById_ShouldReturnOk_WhenPostIsFound()
        {
            // Arrange
            var postId = Guid.NewGuid();

            var mockPost = new PostResponseDto
            {
                Id = postId,
                Title = "Test Post",
                Description = "Test Description"
            };

            var successResponse = new Response<PostResponseDto>
            {
                Succeeded = true,
                Data = mockPost
            };

            // Correct mock setup using the correct return type
            _postServiceMock.Setup(service => service.GetPostByIdAsync(postId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.GetPostById(postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<PostResponseDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Test Post", response.Data.Title);
            Assert.Equal("Test Description", response.Data.Description);
        }


        [Fact]
        public async Task GetPostById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var postId = Guid.NewGuid();

            // Mock the service to throw an exception
            _postServiceMock.Setup(service => service.GetPostByIdAsync(postId))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetPostById(postId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Something went wrong", response.Message);
        }

        [Fact]
        public async Task FilterPosts_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var filter = new FilterPostsRequestDto
            {
                // Set invalid values here that will cause the model state to be invalid.
                Province = "",  // Example invalid field
                MinSize = -1    // Example invalid field
            };

            _controller.ModelState.AddModelError("Province", "Province is required.");

            // Act
            var result = await _controller.FilterPosts(filter);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid filter criteria.", response.Message);
        }

        [Fact]
        public async Task FilterPosts_ShouldReturnNotFound_WhenNoPostsMatchFilterCriteria()
        {
            // Arrange
            var filter = new FilterPostsRequestDto
            {
                Province = "Hanoi",
                District = "District X"
                // Add any filter criteria here
            };

            var noPostsResponse = new Response<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = new List<ListPostsResponseDto>() // No posts
            };

            _postServiceMock.Setup(service => service.FilterPostsAsync(It.IsAny<FilterPostsRequestDto>()))
                .ReturnsAsync(noPostsResponse);

            // Act
            var result = await _controller.FilterPosts(filter);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("No posts found matching the filter criteria.", response.Message);
        }
        [Fact]
        public async Task FilterPosts_ShouldReturnOk_WhenPostsMatchFilterCriteria()
        {
            // Arrange
            var filter = new FilterPostsRequestDto
            {
                Province = "Hanoi",
                District = "District X"
            };

            var filteredPosts = new List<ListPostsResponseDto>
    {
        new ListPostsResponseDto { Id = Guid.NewGuid(), Title = "Post 1" },
        new ListPostsResponseDto { Id = Guid.NewGuid(), Title = "Post 2" }
    };

            var successResponse = new Response<List<ListPostsResponseDto>>
            {
                Succeeded = true,
                Data = filteredPosts
            };

            _postServiceMock.Setup(service => service.FilterPostsAsync(It.IsAny<FilterPostsRequestDto>()))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.FilterPosts(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<ListPostsResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(2, response.Data.Count); // Expecting 2 posts
        }
        [Fact]
        public async Task FilterPosts_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var filter = new FilterPostsRequestDto
            {
                Province = "Hanoi"
            };

            _postServiceMock.Setup(service => service.FilterPostsAsync(It.IsAny<FilterPostsRequestDto>()))
                .ThrowsAsync(new Exception("An unexpected error occurred"));

            // Act
            var result = await _controller.FilterPosts(filter);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: An unexpected error occurred", response.Message);
        }

        [Fact]
        public async Task PushPost_ShouldReturnBadRequest_WhenUserIdIsInvalid()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.Empty; // Invalid userId

            // Act
            var result = await _controller.PushPost(postId, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid user ID.", response.Message);
        }

        [Fact]
        public async Task PushPost_ShouldReturnBadRequest_WhenMembershipServiceNotInitialized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Simulate that the membership service is null or not initialized
            _postServiceMock.Setup(service => service.PushPostOnTopAsync(postId, It.IsAny<DateTime>(), userId))
                .ReturnsAsync(new Response<PostResponseDto> { Succeeded = false, Message = "Membership service not initialized." });

            // Act
            var result = await _controller.PushPost(postId, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Membership service not initialized.", response.Message);
        }

        [Fact]
        public async Task PushPost_ShouldReturnBadRequest_WhenPostNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Simulate that the post is not found
            _postServiceMock.Setup(service => service.PushPostOnTopAsync(postId, It.IsAny<DateTime>(), userId))
                .ReturnsAsync(new Response<PostResponseDto> { Succeeded = false, Message = "Post not found." });

            // Act
            var result = await _controller.PushPost(postId, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Post not found.", response.Message);
        }

        [Fact]
        public async Task PushPost_ShouldReturnOk_WhenPushPostIsSuccessful()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var newPostResponse = new PostResponseDto
            {
                Id = postId,
                Title = "Sample Post",
            };

            var successResponse = new Response<PostResponseDto>
            {
                Succeeded = true,
                Message = "Đẩy bài đăng lên thành công.",
                Data = newPostResponse
            };

            _postServiceMock.Setup(service => service.PushPostOnTopAsync(postId, It.IsAny<DateTime>(), userId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.PushPost(postId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<PostResponseDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Đẩy bài đăng lên thành công.", response.Message);
            Assert.Equal(postId, response.Data.Id);
        }

        [Fact]
        public async Task PushPost_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _postServiceMock.Setup(service => service.PushPostOnTopAsync(postId, It.IsAny<DateTime>(), userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.PushPost(postId, userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

    }
}
