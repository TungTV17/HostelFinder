
using HostelFinder.Application.DTOs.Auth.Requests;
using HostelFinder.Application.DTOs.Auth.Responses;
using HostelFinder.Application.DTOs.Auths.Requests;
using HostelFinder.Application.DTOs.Users;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class AuthControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAuthAccountService> _authAccountServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTest()
        {
            // Mock các dịch vụ cần thiết
            _userServiceMock = new Mock<IUserService>();
            _authAccountServiceMock = new Mock<IAuthAccountService>();
            _authController = new AuthController(_userServiceMock.Object, _authAccountServiceMock.Object, null);
        }

        // Test: Register thành công
        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var request = new CreateUserRequestDto
            {
                Username = "newuser",
                FullName = "New User",
                Password = "StrongPassword123",
                Email = "newuser@example.com",
                Phone = "1234567890"
            };

            var response = new Response<UserDto>
            {
                Succeeded = true,
                Data = new UserDto { Id = Guid.NewGuid(), Username = "newuser" }
            };

            _userServiceMock.Setup(service => service.RegisterUserAsync(It.IsAny<CreateUserRequestDto>()))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultResponse = Assert.IsType<Response<UserDto>>(okResult.Value);
            Assert.True(resultResponse.Succeeded);
        }

        // Test: Register thất bại (Tên người dùng đã tồn tại)
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUsernameExists()
        {
            // Arrange
            var request = new CreateUserRequestDto
            {
                Username = "existinguser",
                FullName = "Existing User",
                Password = "StrongPassword123",
                Email = "user@example.com",
                Phone = "1234567890"
            };

            var response = new Response<UserDto>
            {
                Succeeded = false,
                Message = "Tên người dùng đã tồn tại. Vui lòng nhập tên khác"
            };

            _userServiceMock.Setup(service => service.RegisterUserAsync(It.IsAny<CreateUserRequestDto>()))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultResponse = Assert.IsType<Response<UserDto>>(badRequestResult.Value);
            Assert.False(resultResponse.Succeeded);
            Assert.Contains("Tên người dùng đã tồn tại", resultResponse.Message);
        }

        // Test: Register thất bại (Email đã tồn tại)
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailExists()
        {
            // Arrange
            var request = new CreateUserRequestDto
            {
                Username = "newuser",
                FullName = "New User",
                Password = "StrongPassword123",
                Email = "existingemail@example.com", // Email đã tồn tại
                Phone = "1234567890"
            };

            var response = new Response<UserDto>
            {
                Succeeded = false,
                Message = "Email đã tồn tại. Vui lòng nhập email khác."
            };

            _userServiceMock.Setup(service => service.RegisterUserAsync(It.IsAny<CreateUserRequestDto>()))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultResponse = Assert.IsType<Response<UserDto>>(badRequestResult.Value);
            Assert.False(resultResponse.Succeeded);
            Assert.Contains("Email đã tồn tại", resultResponse.Message);
        }

        // Test: Register thất bại (Số điện thoại đã tồn tại)
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenPhoneNumberExists()
        {
            // Arrange
            var request = new CreateUserRequestDto
            {
                Username = "newuser",
                FullName = "New User",
                Password = "StrongPassword123",
                Email = "newuser@example.com",
                Phone = "1234567890" // Số điện thoại đã tồn tại
            };

            var response = new Response<UserDto>
            {
                Succeeded = false,
                Message = "Số điện thoại đã tồn tại. Vui lòng nhập số điện thoại khác."
            };

            _userServiceMock.Setup(service => service.RegisterUserAsync(It.IsAny<CreateUserRequestDto>()))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultResponse = Assert.IsType<Response<UserDto>>(badRequestResult.Value);
            Assert.False(resultResponse.Succeeded);
            Assert.Contains("Số điện thoại đã tồn tại", resultResponse.Message);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new CreateUserRequestDto
            {
                Username = "newuser",
                FullName = "New User",
                Password = "StrongPassword123",
                Email = "newuser@example.com",
                Phone = "1234567890"
            };

            // Setup: Mô phỏng dịch vụ ném ngoại lệ
            _userServiceMock.Setup(service => service.RegisterUserAsync(It.IsAny<CreateUserRequestDto>()))
                            .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authController.Register(request);

            // Assert
            // Kiểm tra mã trạng thái HTTP là 500 (Internal Server Error)
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);  // Kiểm tra mã trạng thái là 500
            var message = Assert.IsType<string>(objectResult.Value);
            Assert.Contains("Error: Database error", message);  // Kiểm tra thông báo lỗi
        }


        // Test: Register trả về lỗi nếu request body là null
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _authController.Register(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<string>(badRequestResult.Value);
            Assert.Contains("Request body cannot be null", message);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
                UserName = "testuser",
                Password = "testpassword"
            };

            var response = new Response<AuthenticationResponse>
            {
                Succeeded = true,
                Data = new AuthenticationResponse
                {
                    UserName = "testuser",
                    Role = "User",
                    Token = "valid-jwt-token"
                },
                Message = "Đăng nhập thành công"
            };

            _authAccountServiceMock.Setup(service => service.LoginAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<AuthenticationResponse>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);  // Kiểm tra mã trạng thái
            Assert.True(returnValue.Succeeded);  // Kiểm tra nếu đăng nhập thành công
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenUsernameNotFound()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
                UserName = "nonexistentuser",
                Password = "testpassword"
            };

            var response = new Response<AuthenticationResponse>
            {
                Succeeded = false,
                Message = "Tên người dùng không tồn tại. Vui lòng kiểm tra hoặc tạo tài khoản mới."
            };

            _authAccountServiceMock.Setup(service => service.LoginAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<Response<AuthenticationResponse>>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);  // Kiểm tra mã trạng thái
            Assert.False(returnValue.Succeeded);  // Kiểm tra không thành công
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
                UserName = "testuser",
                Password = "wrongpassword"
            };

            var response = new Response<AuthenticationResponse>
            {
                Succeeded = false,
                Message = "Tài khoản hoặc mật khẩu không đúng. Vui lòng kiểm tra lại!"
            };

            _authAccountServiceMock.Setup(service => service.LoginAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<Response<AuthenticationResponse>>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);  // Kiểm tra mã trạng thái
            Assert.False(returnValue.Succeeded);  // Kiểm tra không thành công
        }

        [Fact]
        public async Task Login_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
                UserName = "testuser",
                Password = "testpassword"
            };

            // Setup mock to throw an exception
            _authAccountServiceMock.Setup(service => service.LoginAsync(It.IsAny<AuthenticationRequest>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authController.Login(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // Kiểm tra kiểu trả về
            Assert.Equal(500, statusCodeResult.StatusCode);  // Kiểm tra mã trạng thái là 500
            var returnValue = Assert.IsType<Response<AuthenticationResponse>>(statusCodeResult.Value);  // Kiểm tra kiểu trả về là Response<AuthenticationResponse>
            Assert.Contains("Error: Database error", returnValue.Message);  // Kiểm tra thông báo lỗi
        }



        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                Username = "testuser",
                CurrentPassword = "wrongpassword",
                NewPassword = "newpassword123",
                ConfirmPassword = "newpassword123"
            };

            var response = new Response<string>
            {
                Succeeded = false,
                Message = "Mật khẩu cũ không đúng"
            };

            _authAccountServiceMock.Setup(service => service.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnValue = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.Equal("Mật khẩu cũ không đúng", returnValue.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenNewPasswordAndConfirmPasswordDoNotMatch()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                Username = "testuser",
                CurrentPassword = "oldpassword123",
                NewPassword = "newpassword123",
                ConfirmPassword = "differentpassword123"
            };

            var response = new Response<string>
            {
                Succeeded = false,
                Message = "Mật khẩu mới và xác nhận mật khẩu không giống nhau"
            };

            _authAccountServiceMock.Setup(service => service.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnValue = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.Equal("Mật khẩu mới và xác nhận mật khẩu không giống nhau", returnValue.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenNewPasswordIsSameAsCurrentPassword()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                Username = "testuser",
                CurrentPassword = "password123",
                NewPassword = "password123",
                ConfirmPassword = "password123"
            };

            var response = new Response<string>
            {
                Succeeded = false,
                Message = "Mật khẩu mới không thể trùng mật khẩu cũ"
            };

            _authAccountServiceMock.Setup(service => service.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnValue = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.Equal("Mật khẩu mới không thể trùng mật khẩu cũ", returnValue.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenPasswordIsChangedSuccessfully()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                Username = "testuser",
                CurrentPassword = "oldpassword123",
                NewPassword = "newpassword123",
                ConfirmPassword = "newpassword123"
            };

            var response = new Response<string>
            {
                Succeeded = true,
                Message = "Đổi mật khẩu thành công"
            };

            _authAccountServiceMock.Setup(service => service.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.ChangePassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<Response<string>>(okResult.Value);
            Assert.Equal("Đổi mật khẩu thành công", returnValue.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                Username = "testuser",
                CurrentPassword = "oldpassword123",
                NewPassword = "newpassword123",
                ConfirmPassword = "newpassword123"
            };

            _authAccountServiceMock.Setup(service => service.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authController.ChangePassword(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);  // Kiểm tra mã lỗi 500
            var returnValue = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.Equal("Error: Database error", returnValue.Message);  // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenEmailDoesNotExist()
        {
            // Arrange
            var request = new ForgotPasswordRequest
            {
                Email = "nonexistentuser@example.com"
            };

            var response = new Response<string>
            {
                Succeeded = false,
                Message = "Email không tồn tại. Vui lòng kiểm tra hoặc tạo tài khoản mới."
            };

            _authAccountServiceMock.Setup(service => service.ForgotPasswordAsync(It.IsAny<ForgotPasswordRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.ForgotPassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnValue = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.Equal("Email không tồn tại. Vui lòng kiểm tra hoặc tạo tài khoản mới.", returnValue.Message);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnOk_WhenPasswordIsSentSuccessfully()
        {
            // Arrange
            var request = new ForgotPasswordRequest
            {
                Email = "testuser@example.com"
            };

            var response = new Response<string>
            {
                Succeeded = true,
                Message = "Mật khẩu mới gửi tới email của bạn. Vui lòng check email!"
            };

            _authAccountServiceMock.Setup(service => service.ForgotPasswordAsync(It.IsAny<ForgotPasswordRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.ForgotPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<Response<string>>(okResult.Value);
            Assert.Equal("Mật khẩu mới gửi tới email của bạn. Vui lòng check email!", returnValue.Message);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new ForgotPasswordRequest
            {
                Email = "testuser@example.com"
            };

            _authAccountServiceMock.Setup(service => service.ForgotPasswordAsync(It.IsAny<ForgotPasswordRequest>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authController.ForgotPassword(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);  // Kiểm tra mã lỗi 500
            var returnValue = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.Equal("Error: Database error", returnValue.Message);  // Kiểm tra thông báo lỗi
        }
    }
}