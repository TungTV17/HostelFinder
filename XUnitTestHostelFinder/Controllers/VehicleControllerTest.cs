using HostelFinder.Application.DTOs.Vehicle.Request;
using HostelFinder.Application.DTOs.Vehicle.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class VehicleControllerTest
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;
        private readonly Mock<ITenantRepository> _tenantRepositoryMock;
        private readonly VehicleController _controller;

        public VehicleControllerTest()
        {
            // Initialize the mock object and the controller in the constructor
            _vehicleServiceMock = new Mock<IVehicleService>();
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _controller = new VehicleController(_vehicleServiceMock.Object);
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                               .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetVehicleByTenant(tenantId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);  // Check that it's an ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode);  // Assert the status code is 500
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(internalServerErrorResult.Value);  // Check the type of the response
            Assert.False(response.Succeeded);  // Ensure the result is not successful
            Assert.Contains("Có lỗi xảy ra: Database error", response.Message);  // Ensure the correct error message is included
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnNotFound_WhenNoVehiclesFound()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                               .ReturnsAsync(new Response<IEnumerable<VehicleResponseDto>> { Succeeded = false, Message = "Không tìm thấy xe cho người thuê trọ" });

            // Act
            var result = await _controller.GetVehicleByTenant(tenantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);  // Check that it's a NotFoundObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(notFoundResult.Value);  // Ensure the correct response type
            Assert.False(response.Succeeded);  // Ensure the result is not successful
            Assert.Equal("Không tìm thấy xe cho người thuê trọ", response.Message);  // Check the message is correct
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnOk_WhenVehiclesFound()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = Guid.NewGuid(), TenantId = tenantId, VehicleName = "Vehicle 1" },
                new Vehicle { Id = Guid.NewGuid(), TenantId = tenantId, VehicleName = "Vehicle 2" }
            };

            var vehicleDtos = new List<VehicleResponseDto>
            {
                new VehicleResponseDto { Id = vehicles[0].Id, VehicleName = vehicles[0].VehicleName },
                new VehicleResponseDto { Id = vehicles[1].Id, VehicleName = vehicles[1].VehicleName }
            };

            _vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                               .ReturnsAsync(new Response<IEnumerable<VehicleResponseDto>> { Data = vehicleDtos, Succeeded = true });

            // Act
            var result = await _controller.GetVehicleByTenant(tenantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Check that it's an OkObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(okResult.Value);  // Ensure the response type is correct
            Assert.True(response.Succeeded);  // Ensure the result is successful
            Assert.Equal(2, response.Data.Count());  // Ensure that the correct number of vehicles is returned
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnServiceError_WhenServiceFailsDueToDatabaseIssue()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                              .ThrowsAsync(new Exception("Database connection error"));

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetVehicleByTenant(tenantId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);  // Kiểm tra trả về là ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode);  // Kiểm tra mã lỗi là 500
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(internalServerErrorResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Contains("Database connection error", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnOk_WhenVehiclesFoundWithSpecialCharactersInName()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var vehicles = new List<Vehicle>
    {
        new Vehicle { Id = Guid.NewGuid(), TenantId = tenantId, VehicleName = "Vehicle 1 with @special!" },
        new Vehicle { Id = Guid.NewGuid(), TenantId = tenantId, VehicleName = "Vehicle 2 - *&^%$#@" }
    };

            var vehicleDtos = new List<VehicleResponseDto>
    {
        new VehicleResponseDto { Id = vehicles[0].Id, VehicleName = vehicles[0].VehicleName },
        new VehicleResponseDto { Id = vehicles[1].Id, VehicleName = vehicles[1].VehicleName }
    };

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                              .ReturnsAsync(new Response<IEnumerable<VehicleResponseDto>> { Data = vehicleDtos, Succeeded = true });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetVehicleByTenant(tenantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Kiểm tra trả về là OkObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(okResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded);  // Kết quả thành công
            Assert.Equal(2, response.Data.Count());  // Kiểm tra có 2 xe
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnServiceError_WhenServiceThrowsCustomError()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                              .ThrowsAsync(new Exception("Custom error message"));

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetVehicleByTenant(tenantId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);  // Kiểm tra trả về là ObjectResult
            Assert.Equal(500, objectResult.StatusCode);  // Kiểm tra mã lỗi là 500
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(objectResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Contains("Custom error message", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetVehicleByTenantAsync(tenantId))
                              .ReturnsAsync(new Response<IEnumerable<VehicleResponseDto>> { Succeeded = false, Message = "No vehicles found" });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetVehicleByTenant(tenantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);  // Kiểm tra trả về là NotFoundObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(notFoundResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Equal("No vehicles found", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task GetVehicleByTenant_ShouldReturnBadRequest_WhenTenantIdIsInvalid()
        {
            // Arrange
            var invalidTenantId = Guid.Empty;  // Một Guid không hợp lệ (Guid.Empty)

            var vehicleServiceMock = new Mock<IVehicleService>();
            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetVehicleByTenant(invalidTenantId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Contains("Invalid tenantId", response.Message);  // Kiểm tra thông báo lỗi về tenantId không hợp lệ
        }

        [Fact]
        public async Task AddVehicle_ShouldReturnOk_WhenVehicleIsAddedSuccessfully()
        {
            // Arrange
            var request = new AddVehicleDto { TenantId = Guid.NewGuid(), Image = new FormFile(new MemoryStream(), 0, 0, "", "vehicle.jpg") };
            var tenant = new Tenant { Id = request.TenantId, FullName = "John Doe" };

            _tenantRepositoryMock.Setup(x => x.GetByIdAsync(request.TenantId)).ReturnsAsync(tenant);
            var vehicleDto = new VehicleResponseDto { Id = Guid.NewGuid(), VehicleName = "Car", ImageUrl = "http://aws.com/image" };

            _vehicleServiceMock.Setup(x => x.AddVehicleAsync(request)).ReturnsAsync(new Response<VehicleResponseDto> { Data = vehicleDto, Succeeded = true, Message = "Thêm xe thành công" });

            var controller = new VehicleController(_vehicleServiceMock.Object);

            // Act
            var result = await controller.AddVehicle(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Kiểm tra trả về là OkObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(okResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded);  // Kết quả thành công
            Assert.Equal("Thêm xe thành công", response.Message);  // Kiểm tra thông báo thành công
        }

        [Fact]
        public async Task AddVehicle_ShouldReturnBadRequest_WhenTenantNotFound()
        {
            // Arrange
            var invalidTenantId = Guid.NewGuid();  // Giả sử tenant không tồn tại
            var request = new AddVehicleDto
            {
                TenantId = invalidTenantId,
                VehicleName = "Car 1",
                Image = new FormFile(null, 0, 0, null, null)
            };

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.AddVehicleAsync(It.IsAny<AddVehicleDto>()))
                              .ReturnsAsync(new Response<VehicleResponseDto>
                              {
                                  Succeeded = false,
                                  Message = "Không tìm thấy người thuê trọ"
                              });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.AddVehicle(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kiểm tra kết quả không thành công
            Assert.Equal("Không tìm thấy người thuê trọ", response.Message);  // Kiểm tra thông báo lỗi đúng
        }


        [Fact]
        public async Task AddVehicle_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            AddVehicleDto request = null; // Null request sẽ trigger BadRequest trong controller

            var controller = new VehicleController(_vehicleServiceMock.Object);

            // Act
            var result = await controller.AddVehicle(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Kiểm tra trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(badRequestResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kiểm tra kết quả không thành công
            Assert.Equal("Request body cannot be empty.", response.Message);  // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task AddVehicle_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var request = new AddVehicleDto { TenantId = Guid.NewGuid(), Image = new FormFile(new MemoryStream(), 0, 0, "", "vehicle.jpg") };
            var tenant = new Tenant { Id = request.TenantId, FullName = "John Doe" };

            _tenantRepositoryMock.Setup(x => x.GetByIdAsync(request.TenantId)).ReturnsAsync(tenant);
            _vehicleServiceMock.Setup(x => x.AddVehicleAsync(It.IsAny<AddVehicleDto>())).ThrowsAsync(new Exception("An unexpected error occurred"));

            var controller = new VehicleController(_vehicleServiceMock.Object);

            // Act
            var result = await controller.AddVehicle(request);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result); // Kiểm tra trả về ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode); // Kiểm tra mã lỗi 500
            var response = Assert.IsType<Response<VehicleResponseDto>>(internalServerErrorResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded); // Kết quả không thành công
            Assert.Contains("An unexpected error occurred", response.Errors.First()); // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task GetVehicleById_ShouldReturnOk_WhenVehicleFound()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Vehicle
            {
                Id = vehicleId,
                VehicleName = "Vehicle 1",
                TenantId = Guid.NewGuid(),
                ImageUrl = "image.jpg"
            };

            var vehicleDto = new VehicleResponseDto
            {
                Id = vehicle.Id,
                VehicleName = vehicle.VehicleName,
                ImageUrl = vehicle.ImageUrl
            };

            _vehicleServiceMock.Setup(service => service.GetVehicleByIdAsync(vehicleId))
                               .ReturnsAsync(new Response<VehicleResponseDto> { Data = vehicleDto, Succeeded = true });

            // Act
            var result = await _controller.GetVehicleById(vehicleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Kiểm tra trả về là OkObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(okResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded);  // Kiểm tra kết quả thành công
            Assert.Equal(vehicleId, response.Data.Id);  // Kiểm tra ID xe đúng
        }

        [Fact]
        public async Task GetVehicleById_ShouldReturnNotFound_WhenVehicleNotFound()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleServiceMock.Setup(service => service.GetVehicleByIdAsync(vehicleId))
                               .ReturnsAsync(new Response<VehicleResponseDto> { Succeeded = false, Message = "Không tìm thấy xe" });

            // Act
            var result = await _controller.GetVehicleById(vehicleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);  // Kiểm tra trả về là NotFoundObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(notFoundResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kiểm tra kết quả không thành công
            Assert.Equal("Không tìm thấy xe", response.Message);  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task GetVehicleById_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleServiceMock.Setup(service => service.GetVehicleByIdAsync(vehicleId))
                               .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetVehicleById(vehicleId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);  // Kiểm tra trả về là ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode);  // Kiểm tra mã lỗi là 500
            var response = Assert.IsType<Response<VehicleResponseDto>>(internalServerErrorResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kiểm tra kết quả không thành công
            Assert.Contains("Có lỗi xảy ra: Database error", response.Errors.First());  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task GetAllVehicles_ShouldReturnOk_WhenVehiclesFound()
        {
            // Arrange
            var vehicles = new List<Vehicle>
    {
        new Vehicle { Id = Guid.NewGuid(), VehicleName = "Vehicle 1" },
        new Vehicle { Id = Guid.NewGuid(), VehicleName = "Vehicle 2" }
    };

            var vehicleDtos = new List<VehicleResponseDto>
    {
        new VehicleResponseDto { Id = vehicles[0].Id, VehicleName = vehicles[0].VehicleName },
        new VehicleResponseDto { Id = vehicles[1].Id, VehicleName = vehicles[1].VehicleName }
    };

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetAllVehiclesAsync())
                              .ReturnsAsync(new Response<IEnumerable<VehicleResponseDto>> { Data = vehicleDtos, Succeeded = true });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetAllVehicles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Kiểm tra trả về là OkObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(okResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded);  // Kiểm tra kết quả thành công
            Assert.Equal(2, response.Data.Count());  // Kiểm tra có 2 xe
        }

        [Fact]
        public async Task GetAllVehicles_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetAllVehiclesAsync())
                              .ThrowsAsync(new Exception("Database error"));

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetAllVehicles();

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);  // Kiểm tra trả về là ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode);  // Kiểm tra mã lỗi là 500
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(internalServerErrorResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded);  // Kết quả không thành công
            Assert.Contains("Có lỗi xảy ra: Database error", response.Errors.First());  // Kiểm tra thông báo lỗi đúng
        }

        [Fact]
        public async Task GetAllVehicles_ShouldReturnOk_WhenNoVehiclesFound()
        {
            // Arrange
            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.GetAllVehiclesAsync())
                              .ReturnsAsync(new Response<IEnumerable<VehicleResponseDto>> { Data = new List<VehicleResponseDto>(), Succeeded = true });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.GetAllVehicles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Kiểm tra trả về là OkObjectResult
            var response = Assert.IsType<Response<IEnumerable<VehicleResponseDto>>>(okResult.Value);  // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded);  // Kết quả thành công
            Assert.Empty(response.Data);  // Kiểm tra danh sách xe rỗng
        }

        [Fact]
        public async Task UpdateVehicle_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var addVehicleDto = new AddVehicleDto { VehicleName = "Updated Vehicle", Image = null };

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.UpdateVehicleAsync(vehicleId, addVehicleDto))
                              .ThrowsAsync(new Exception("Database error"));

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.UpdateVehicle(vehicleId, addVehicleDto);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result); // Kiểm tra trả về là ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode); // Kiểm tra mã lỗi là 500
            var response = Assert.IsType<Response<VehicleResponseDto>>(internalServerErrorResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded); // Kiểm tra Succeeded là false
            Assert.Contains("Có lỗi xảy ra: Database error", response.Errors.First()); // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task UpdateVehicle_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            AddVehicleDto addVehicleDto = null;  // Giả sử request là null

            var controller = new VehicleController(new Mock<IVehicleService>().Object);

            // Act
            var result = await controller.UpdateVehicle(vehicleId, addVehicleDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Kiểm tra trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(badRequestResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded); // Kiểm tra Succeeded là false
            Assert.Equal("Request body cannot be empty.", response.Message); // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task UpdateVehicle_ShouldReturnBadRequest_WhenVehicleNotFound()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var addVehicleDto = new AddVehicleDto { VehicleName = "Updated Vehicle", Image = null };

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.UpdateVehicleAsync(vehicleId, addVehicleDto))
                              .ReturnsAsync(new Response<VehicleResponseDto> { Succeeded = false, Message = "Không tìm thấy xe" });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.UpdateVehicle(vehicleId, addVehicleDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Kiểm tra trả về là BadRequestObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(badRequestResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded); // Kiểm tra Succeeded là false
            Assert.Equal("Không tìm thấy xe", response.Message); // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task UpdateVehicle_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicleDto = new VehicleResponseDto { Id = vehicleId, VehicleName = "Updated Vehicle" };
            var addVehicleDto = new AddVehicleDto { VehicleName = "Updated Vehicle", Image = null };

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.UpdateVehicleAsync(vehicleId, addVehicleDto))
                              .ReturnsAsync(new Response<VehicleResponseDto> { Data = vehicleDto, Succeeded = true, Message = "Cập nhật xe thành công" });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.UpdateVehicle(vehicleId, addVehicleDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Kiểm tra trả về là OkObjectResult
            var response = Assert.IsType<Response<VehicleResponseDto>>(okResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded); // Kiểm tra Succeeded là true
            Assert.Equal("Cập nhật xe thành công", response.Message); // Kiểm tra thông báo thành công
        }

        [Fact]
        public async Task DeleteVehicle_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var vehicleId = Guid.NewGuid(); // ID ngẫu nhiên của xe

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.DeleteVehicleAsync(vehicleId))
                              .ThrowsAsync(new Exception("Database error")); // Mô phỏng lỗi khi xóa

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.DeleteVehicle(vehicleId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result); // Kiểm tra trả về là ObjectResult
            Assert.Equal(500, internalServerErrorResult.StatusCode); // Kiểm tra mã lỗi là 500
            var response = Assert.IsType<Response<bool>>(internalServerErrorResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded); // Kiểm tra Succeeded là false
            Assert.Contains("Có lỗi xảy ra: Database error", response.Errors.First()); // Kiểm tra thông báo lỗi
        }

        [Fact]
        public async Task DeleteVehicle_ShouldReturnOk_WhenVehicleDeletedSuccessfully()
        {
            // Arrange
            var vehicleId = Guid.NewGuid(); // ID của xe cần xóa

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.DeleteVehicleAsync(vehicleId))
                              .ReturnsAsync(new Response<bool> { Data = true, Succeeded = true, Message = "Xóa xe thành công" });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.DeleteVehicle(vehicleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Kiểm tra trả về là Ok
            var response = Assert.IsType<Response<bool>>(okResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.True(response.Succeeded); // Kiểm tra Succeeded là true
            Assert.Contains("Xóa xe thành công", response.Message); // Kiểm tra thông báo thành công
        }

        [Fact]
        public async Task DeleteVehicle_ShouldReturnNotFound_WhenVehicleNotFound()
        {
            // Arrange
            var vehicleId = Guid.NewGuid(); // Sử dụng ID ngẫu nhiên để giả lập xe không tồn tại

            var vehicleServiceMock = new Mock<IVehicleService>();
            vehicleServiceMock.Setup(service => service.DeleteVehicleAsync(vehicleId))
                              .ReturnsAsync(new Response<bool> { Succeeded = false, Message = "Không tìm thấy xe" });

            var controller = new VehicleController(vehicleServiceMock.Object);

            // Act
            var result = await controller.DeleteVehicle(vehicleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); // Kiểm tra trả về là NotFound
            var response = Assert.IsType<Response<bool>>(notFoundResult.Value); // Kiểm tra kiểu dữ liệu trả về
            Assert.False(response.Succeeded); // Kiểm tra Succeeded là false
            Assert.Contains("Không tìm thấy xe", response.Message); // Kiểm tra thông báo lỗi
        }


    }
}
