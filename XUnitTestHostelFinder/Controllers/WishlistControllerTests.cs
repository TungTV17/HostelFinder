using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.DTOs.Wishlist.Request;
using HostelFinder.Application.DTOs.Wishlist.Response;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HostelFinder.UnitTests.Controllers
{
    public class WishlistControllerTests
    {
        private readonly WishlistController _controller;
        private readonly Mock<IWishlistService> _wishlistServiceMock;

        public WishlistControllerTests()
        {
            _wishlistServiceMock = new Mock<IWishlistService>();
            _controller = new WishlistController(_wishlistServiceMock.Object);
        }

        [Fact]
        public async Task AddRoomToWishlist_ReturnsBadRequest_WhenPostIdOrUserIdIsInvalid()
        {
            // Arrange
            var request = new AddPostToWishlistRequestDto
            {
                PostId = Guid.Empty,
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await _controller.AddRoomToWishlist(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Ensure it matches BadRequestObjectResult
            var response = Assert.IsType<Response<bool>>(badRequestResult.Value); // Ensure the response is of the correct type
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid Post ID or User ID.", response.Message);
        }

        [Fact]
        public async Task AddRoomToWishlist_ReturnsBadRequest_WhenAdditionFails()
        {
            // Arrange
            var request = new AddPostToWishlistRequestDto
            {
                PostId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var mockResponse = new Response<bool>
            {
                Succeeded = false,
                Errors = new List<string> { "Failed to add room to wishlist." }
            };

            _wishlistServiceMock.Setup(service => service.AddPostToWishlistAsync(request))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddRoomToWishlist(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<bool>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Failed to add room to wishlist.", response.Errors);
        }

        [Fact]
        public async Task AddRoomToWishlist_ReturnsOk_WhenAdditionSucceeds()
        {
            // Arrange
            var request = new AddPostToWishlistRequestDto
            {
                PostId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var mockResponse = new Response<bool>
            {
                Succeeded = true,
                Message = "Thêm vào danh sách yêu thích thành công." // Set the expected message
            };

            _wishlistServiceMock.Setup(service => service.AddPostToWishlistAsync(request))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddRoomToWishlist(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Thêm vào danh sách yêu thích thành công.", response.Message);
        }


        //[Fact]
        //public async Task GetWishlistByUserId_ReturnsOkResult_WhenWishlistExists()
        //{
        //    // Arrange
        ////    var userId = Guid.NewGuid();
        ////    var mockWishlistResponse = new WishlistResponseDto
        ////    {
        ////        WishlistId = Guid.NewGuid(),
        ////        Posts = new List<PostResponseDto>
        ////{
        ////    new PostResponseDto { Id = Guid.NewGuid(), Title = "Post 1" },
        ////    new PostResponseDto { Id = Guid.NewGuid(), Title = "Post 2" }
        ////}
        ////    };

        //    var response = new Response<WishlistResponseDto>(mockWishlistResponse);

        //    _wishlistServiceMock.Setup(service => service.GetWishlistByUserIdAsync(userId))
        //        .ReturnsAsync(response);

        //    // Act
        //    var result = await _controller.GetWishlistByUserId(userId);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var responseData = Assert.IsType<Response<WishlistResponseDto>>(okResult.Value);
        //    Assert.True(responseData.Succeeded);
        //    Assert.Equal(2, responseData.Data.Posts.Count);
        //}


        [Fact]
        public async Task GetWishlistByUserId_ReturnsBadRequest_WhenUserIdIsInvalid()
        {
            // Arrange
            var invalidUserId = Guid.Empty;

            // Act
            var result = await _controller.GetWishlistByUserId(invalidUserId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<WishlistResponseDto>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid User ID.", response.Message);
        }

        [Fact]
        public async Task GetWishlistByUserId_ReturnsNotFound_WhenWishlistDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var response = new Response<WishlistResponseDto>(null, "Wishlist not found")
            {
                Succeeded = false
            };

            _wishlistServiceMock.Setup(service => service.GetWishlistByUserIdAsync(userId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetWishlistByUserId(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<WishlistResponseDto>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Wishlist not found", responseData.Message);
        }

        [Fact]
        public async Task GetWishlistByUserId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _wishlistServiceMock.Setup(service => service.GetWishlistByUserIdAsync(userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetWishlistByUserId(userId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var response = Assert.IsType<Response<WishlistResponseDto>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task DeleteWishlist_ReturnsOkResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            var wishlistId = Guid.NewGuid();
            var mockResponse = new Response<bool>(true, "Wishlist item deleted successfully.");

            _wishlistServiceMock.Setup(service => service.DeleteRoomFromWishlistAsync(wishlistId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.DeleteWishlist(wishlistId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal("Wishlist item deleted successfully.", responseData.Message);
        }

        [Fact]
        public async Task DeleteWishlist_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.DeleteWishlist(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(badRequestResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Invalid ID.", responseData.Message);
        }

        [Fact]
        public async Task DeleteWishlist_ReturnsNotFound_WhenWishlistDoesNotExist()
        {
            // Arrange
            var wishlistId = Guid.NewGuid();
            var mockResponse = new Response<bool>
            {
                Succeeded = false,
                Message = "Wishlist not found"
            };

            _wishlistServiceMock.Setup(service => service.DeleteRoomFromWishlistAsync(wishlistId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.DeleteWishlist(wishlistId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Wishlist not found", responseData.Message);
        }


        [Fact]
        public async Task DeleteWishlist_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var wishlistId = Guid.NewGuid();

            _wishlistServiceMock.Setup(service => service.DeleteRoomFromWishlistAsync(wishlistId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.DeleteWishlist(wishlistId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseData = Assert.IsType<Response<bool>>(objectResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", responseData.Message);
        }

        [Fact]
        public async Task AddRoomToWishlist_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new AddPostToWishlistRequestDto
            {
                PostId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _wishlistServiceMock.Setup(service => service.AddPostToWishlistAsync(request))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.AddRoomToWishlist(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<bool>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

    }
}
