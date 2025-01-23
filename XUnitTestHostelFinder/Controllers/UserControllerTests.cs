using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.DTOs.Users.Response;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetListUser_ReturnsOkResult_WhenUsersExist()
        {
            // Arrange
            var mockUsers = new List<UserDto>
    {
        new UserDto { Id = Guid.NewGuid(), Username = "User1" },
        new UserDto { Id = Guid.NewGuid(), Username = "User2" }
    };

            var response = new Response<List<UserDto>>(mockUsers);

            _userServiceMock.Setup(service => service.GetAllUsersAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetListUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<UserDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal(2, responseData.Data.Count);
        }


        [Fact]
        public async Task GetListUser_ReturnsNotFound_WhenNoUsersExist()
        {
            // Arrange
            var response = new Response<List<UserDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "No users found." }
            };

            _userServiceMock.Setup(service => service.GetAllUsersAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetListUser();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<List<UserDto>>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("No users found.", responseData.Message);
        }


        [Fact]
        public async Task GetUserById_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockResponse = new Response<UserProfileResponse>
            {
                Data = new UserProfileResponse
                {
                    Id = userId,
                    Username = "johndoe",
                    FullName = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890",
                    AvatarUrl = "https://example.com/avatar.jpg"
                },
                Succeeded = true
            };

            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotNull(response.Data);
            Assert.Equal(userId, response.Data.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.GetUserById(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid user ID.", response.Message);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockResponse = new Response<UserProfileResponse>
            {
                Data = null,
                Succeeded = false,
                Errors = new List<string> { "User not found." }
            };

            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("User not found.", response.Errors);
        }

        [Fact]
        public async Task GetUserById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<UserProfileResponse>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOkResult_WhenUpdateSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updatedUser = new UserDto
            {
                Username = "updatedUsername",
                Email = "updated@example.com",
                Phone = "123456789",
                AvatarUrl = null,
                Role = HostelFinder.Domain.Enums.UserRole.User,
                IsActive = true
            };

            var mockResponse = new Response<UserDto>
            {
                Data = updatedUser,
                Succeeded = true
            };

            //_userServiceMock
            //    .Setup(service => service.UpdateUserAsync(userId, It.IsAny<UpdateUserRequestDto>()))
            //    .ReturnsAsync(mockResponse);

            //// Act
            //var result = await _controller.UpdateUser(userId, new UpdateUserRequestDto());

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //var returnValue = Assert.IsType<Response<UserDto>>(okResult.Value);
            //Assert.True(returnValue.Succeeded);
            //Assert.Equal("updatedUsername", returnValue.Data.Username);
            //Assert.Equal("updated@example.com", returnValue.Data.Email);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateUserDto = new UpdateUserRequestDto
            {
                Username = "updatedUser",
                Email = "updated@example.com",
                Phone = "123456789"
            };

            var mockResponse = new Response<UserDto>
            {
                Succeeded = false,
                Errors = new List<string> { "Update failed" }
            };

            //_userServiceMock
            //    .Setup(service => service.UpdateUserAsync(userId, updateUserDto))
            //    .ReturnsAsync(mockResponse);

            //// Act
            //var result = await _controller.UpdateUser(userId, updateUserDto);

            // Assert
            //var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); 
            //var returnValue = Assert.IsType<Response<UserDto>>(notFoundResult.Value);
            //Assert.False(returnValue.Succeeded);
            //Assert.Contains("Update failed", returnValue.Errors);
        }


        [Fact]
        public async Task UnActiveUser_ReturnsOkResult_WhenDeactivationSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockResponse = new Response<bool>
            {
                Data = true,
                Succeeded = true
            };

            _userServiceMock
                .Setup(service => service.UnActiveUserAsync(userId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.UnActiveUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.True(returnValue.Data);
        }

        [Fact]
        public async Task UnActiveUser_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(service => service.UnActiveUserAsync(userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UnActiveUser(userId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var response = Assert.IsType<Response<bool>>(objectResult.Value); // Updated to Response<bool>
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
            Assert.False(response.Data); // Validate that the Data property is false
        }

        [Fact]
        public async Task UnActiveUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockResponse = new Response<bool>
            {
                Data = false,
                Succeeded = false,
                Message = "Người dùng không tồn tại."
            };

            _userServiceMock
                .Setup(service => service.UnActiveUserAsync(userId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.UnActiveUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<Response<bool>>(notFoundResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Người dùng không tồn tại.", returnValue.Message);
        }

        [Fact]
        public async Task GetListUser_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _userServiceMock.Setup(service => service.GetAllUsersAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetListUser();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var response = Assert.IsType<Response<List<UserDto>>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task GetUserById_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Id", "Invalid user ID");

            // Act
            var result = await _controller.GetUserById(Guid.Empty); // Simulate invalid ID

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid user ID.", response.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequestDto
            {
                Username = "usererror",
                Email = "usererror@example.com",
                Phone = "1122334455",
                FullName = "User Error"
            };

            _userServiceMock.Setup(service => service.UpdateUserAsync(userId, request, null))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UpdateUser(userId, request, null);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var response = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task UnActiveUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Id", "Invalid user ID");

            // Act
            var result = await _controller.UnActiveUser(Guid.Empty); // Simulate an invalid Guid

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsType<Response<bool>>(badRequestResult.Value); // Expecting Response<bool>
            Assert.False(responseData.Succeeded);
            Assert.Equal("Invalid model state.", responseData.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidRequest = new UpdateUserRequestDto();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            // Act
            var result = await _controller.UpdateUser(Guid.NewGuid(), invalidRequest, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid model state.", response.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequestDto
            {
                Username = "newuser",
                Email = "newemail@example.com",
                Phone = "123456789",
                FullName = "New User"
            };

            var response = new Response<UserDto>("Người dùng không tồn tại.")
            {
                Succeeded = false
            };

            _userServiceMock.Setup(service => service.UpdateUserAsync(userId, request, null))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateUser(userId, request, null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Người dùng không tồn tại.", returnValue.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequestDto
            {
                Username = "updateduser",
                Email = "updatedemail@example.com",
                Phone = "987654321",
                FullName = "Updated User"
            };

            var response = new Response<UserDto>(new UserDto
            {
                Id = userId,
                Username = "updateduser",
                Email = "updatedemail@example.com",
                Phone = "987654321",
                FullName = "Updated User"
            });

            _userServiceMock.Setup(service => service.UpdateUserAsync(userId, request, null))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateUser(userId, request, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<UserDto>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("updateduser", returnValue.Data.Username);
            Assert.Equal("updatedemail@example.com", returnValue.Data.Email);
        }

        [Fact]
        public async Task GetUserByHostelId_ReturnsBadRequest_WhenHostelIdIsInvalid()
        {
            // Act
            var result = await _controller.GetUserByHostelId(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid hostel ID.", response.Message);
        }

        [Fact]
        public async Task GetUserByHostelId_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var mockResponse = new Response<UserProfileResponse>
            {
                Succeeded = false,
                Message = "User not found for the given hostel ID."
            };

            _userServiceMock.Setup(service => service.GetUserByHostelIdAsync(hostelId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetUserByHostelId(hostelId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("User not found for the given hostel ID.", response.Message);
        }

        [Fact]
        public async Task GetUserByHostelId_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var mockUser = new UserProfileResponse
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "johndoe@example.com",
                Phone = "123456789",
                AvatarUrl = "https://example.com/avatar.jpg"
            };
            var mockResponse = new Response<UserProfileResponse>(mockUser);

            _userServiceMock.Setup(service => service.GetUserByHostelIdAsync(hostelId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetUserByHostelId(hostelId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<UserProfileResponse>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(mockUser.FullName, response.Data.FullName);
            Assert.Equal(mockUser.Email, response.Data.Email);
        }

        [Fact]
        public async Task GetUserByHostelId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var hostelId = Guid.NewGuid();

            _userServiceMock.Setup(service => service.GetUserByHostelIdAsync(hostelId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetUserByHostelId(hostelId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<UserProfileResponse>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

    }
}
