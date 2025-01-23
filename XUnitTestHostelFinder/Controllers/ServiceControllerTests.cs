using HostelFinder.Application.DTOs.Service.Request;
using HostelFinder.Application.DTOs.Service.Response;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Enums;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class ServiceControllerTests
    {
        private readonly Mock<IServiceService> _serviceServiceMock;
        private readonly ServiceController _controller;

        public ServiceControllerTests()
        {
            _serviceServiceMock = new Mock<IServiceService>();
            _controller = new ServiceController(_serviceServiceMock.Object);
        }

        [Fact]
        public async Task GetAllServices_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _serviceServiceMock.Setup(service => service.GetAllServicesAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetAllServices();

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);

            var response = Assert.IsType<Response<string>>(internalServerErrorResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task GetAllServices_ReturnsNotFound_WhenNoServicesExist()
        {
            // Arrange
            var mockResponse = new Response<List<ServiceResponseDTO>>
            {
                Succeeded = true,
                Data = new List<ServiceResponseDTO>()
            };

            _serviceServiceMock.Setup(service => service.GetAllServicesAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllServices();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("No services available.", response.Message);
        }

        [Fact]
        public async Task GetAllServices_ReturnsOkResult_WhenServicesExist()
        {
            // Arrange
            var mockServices = new List<ServiceResponseDTO>
    {
        new ServiceResponseDTO { Id = Guid.NewGuid(), ServiceName = "Service 1" },
        new ServiceResponseDTO { Id = Guid.NewGuid(), ServiceName = "Service 2" }
    };

            var mockResponse = new Response<List<ServiceResponseDTO>>
            {
                Succeeded = true,
                Data = mockServices
            };

            _serviceServiceMock.Setup(service => service.GetAllServicesAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllServices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<ServiceResponseDTO>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEmpty(response.Data);
            Assert.Equal(2, response.Data.Count);
        }

        [Fact]
        public async Task GetServiceByIdAsync_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var serviceId = Guid.NewGuid();

            // Mocking the service to simulate a "not found" scenario
            _serviceServiceMock.Setup(service => service.GetServiceByIdAsync(serviceId))
                .ReturnsAsync(new Response<ServiceResponseDTO>("Service not found."));

            // Act
            var result = await _controller.GetServiceById(serviceId);

            // Assert
            // Check that the result is NotFound
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert that the response contains the expected message
            var response = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Service not found.", response.Message);
        }

        [Fact]
        public async Task GetServiceByIdAsync_ShouldReturnService_WhenServiceExists()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var mockService = new ServiceResponseDTO
            {
                Id = serviceId,
                ServiceName = "Test Service"
            };

            var mockResponse = new Response<ServiceResponseDTO>
            {
                Succeeded = true,
                Data = mockService
            };

            _serviceServiceMock.Setup(service => service.GetServiceByIdAsync(serviceId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceById(serviceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<ServiceResponseDTO>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Test Service", response.Data.ServiceName);
        }

        [Fact]
        public async Task GetServiceByIdAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var serviceId = Guid.NewGuid();

            _serviceServiceMock.Setup(service => service.GetServiceByIdAsync(serviceId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetServiceById(serviceId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);

            var response = Assert.IsType<Response<string>>(internalServerErrorResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task AddService_ShouldReturnOk_WhenServiceIsAddedSuccessfully()
        {
            // Arrange
            var serviceCreateRequestDTO = new ServiceCreateRequestDTO
            {
                ServiceName = "Laundry Service",
                HostelId = Guid.NewGuid(),
                ChargingMethod = ChargingMethod.PerPerson
            };

            var serviceResponseDTO = new ServiceResponseDTO
            {
                Id = Guid.NewGuid(),
                ServiceName = "Laundry Service"
            };

            _serviceServiceMock.Setup(service => service.AddServiceAsync(serviceCreateRequestDTO))
                .ReturnsAsync(new Response<ServiceResponseDTO>
                {
                    Succeeded = true,
                    Data = serviceResponseDTO,
                    Message = "Dịch vụ Laundry Service đã được thêm thành công."
                });

            // Act
            var result = await _controller.AddService(serviceCreateRequestDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<ServiceResponseDTO>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Laundry Service", response.Data.ServiceName);
        }

        [Fact]
        public async Task AddService_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var serviceCreateRequestDTO = new ServiceCreateRequestDTO
            {
                ServiceName = "Laundry Service",
                HostelId = Guid.NewGuid(),
                ChargingMethod = ChargingMethod.PerPerson
            };

            // Mock the service to throw an exception
            _serviceServiceMock.Setup(service => service.AddServiceAsync(serviceCreateRequestDTO))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.AddService(serviceCreateRequestDTO);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Expecting ObjectResult, not BadRequest
            Assert.Equal(500, objectResult.StatusCode); // Ensure the status code is 500
            var response = Assert.IsType<Response<string>>(objectResult.Value);  // Ensure Response<string> type
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message); // Check the error message
        }

        [Fact]
        public async Task UpdateService_ShouldReturnNoContent_WhenServiceIsUpdatedSuccessfully()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var serviceUpdateRequestDTO = new ServiceUpdateRequestDTO
            {
                ServiceName = "Updated Service",
                ChargingMethod = ChargingMethod.PerPerson,
            };

            var mockResponse = new Response<ServiceResponseDTO>
            {
                Succeeded = true,
                Message = "Service updated successfully."
            };

            _serviceServiceMock.Setup(service => service.UpdateServiceAsync(serviceId, serviceUpdateRequestDTO))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.UpdateService(serviceId, serviceUpdateRequestDTO);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task UpdateService_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var serviceUpdateRequestDTO = new ServiceUpdateRequestDTO
            {
                ServiceName = "Updated Service",
                ChargingMethod = ChargingMethod.PerPerson,
            };

            var mockResponse = new Response<ServiceResponseDTO>
            {
                Succeeded = false,
                Message = "Service not found."
            };

            _serviceServiceMock.Setup(service => service.UpdateServiceAsync(serviceId, serviceUpdateRequestDTO))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.UpdateService(serviceId, serviceUpdateRequestDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Service not found.", response.Message);
        }

        [Fact]
        public async Task UpdateService_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var serviceUpdateRequestDTO = new ServiceUpdateRequestDTO
            {
                ServiceName = "Updated Service",
                ChargingMethod = ChargingMethod.PerPerson,
            };

            _serviceServiceMock.Setup(service => service.UpdateServiceAsync(serviceId, serviceUpdateRequestDTO))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UpdateService(serviceId, serviceUpdateRequestDTO);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, badRequestResult.StatusCode);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task DeleteService_ShouldReturnOk_WhenServiceIsDeletedSuccessfully()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var mockResponse = new Response<string>
            {
                Succeeded = true,
                Message = "Service deleted successfully."
            };

            _serviceServiceMock.Setup(service => service.DeleteServiceAsync(serviceId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.DeleteService(serviceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<string>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Service deleted successfully.", response.Message);
        }

        [Fact]
        public async Task DeleteService_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var mockResponse = new Response<string>
            {
                Succeeded = false,
                Message = "Service not found."
            };

            _serviceServiceMock.Setup(service => service.DeleteServiceAsync(serviceId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.DeleteService(serviceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Service not found.", response.Message);
        }

        [Fact]
        public async Task DeleteService_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            _serviceServiceMock.Setup(service => service.DeleteServiceAsync(serviceId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.DeleteService(serviceId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task GetServiceByHostel_ShouldReturnOk_WhenServicesAreFound()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var mockResponse = new Response<List<HostelServiceResponseDto>>
            {
                Succeeded = true,
                Message = "Lấy danh sách dịch vụ trong phòng trọ thành công",
                Data = new List<HostelServiceResponseDto>
            {
                new HostelServiceResponseDto { /* Mock your DTO properties here */ }
            }
            };

            _serviceServiceMock.Setup(service => service.GetAllServiceByHostelAsync(hostelId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceByHostel(hostelId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<HostelServiceResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Lấy danh sách dịch vụ trong phòng trọ thành công", response.Message);
        }

        [Fact]
        public async Task GetServiceByHostel_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            _serviceServiceMock.Setup(service => service.GetAllServiceByHostelAsync(hostelId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetServiceByHostel(hostelId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

    }
}
