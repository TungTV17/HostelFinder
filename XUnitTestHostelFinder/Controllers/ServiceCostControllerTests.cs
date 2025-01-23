using HostelFinder.Application.DTOs.ServiceCost.Request;
using HostelFinder.Application.DTOs.ServiceCost.Responses;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using HostelFinder.WebApi.Controllers;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Enums;

namespace XUnitTestHostelFinder.Controllers
{
    public class ServiceCostControllerTests
    {
        private readonly Mock<IServiceCostService> _serviceCostServiceMock;
        private readonly ServiceCostController _controller;

        public ServiceCostControllerTests()
        {
            _serviceCostServiceMock = new Mock<IServiceCostService>();
            _controller = new ServiceCostController(_serviceCostServiceMock.Object);
        }



        [Fact]
        public async Task GetServiceCost_ReturnsOkResult_WhenServiceCostExists()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var mockResponse = new Response<ServiceCostResponseDto>
            {
                Data = new ServiceCostResponseDto { ServiceName = "Test Service" },
                Succeeded = true
            };

            _serviceCostServiceMock
                .Setup(service => service.GetByIdAsync(serviceCostId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCost(serviceCostId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("Test Service", returnValue.Data.ServiceName);
        }


        [Fact]
        public async Task GetServiceCost_ReturnsNotFound_WhenServiceCostDoesNotExist()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var mockResponse = new Response<ServiceCostResponseDto>
            {
                Data = null,
                Succeeded = false,
                Message = "Service cost not found."
            };

            _serviceCostServiceMock
                .Setup(service => service.GetByIdAsync(serviceCostId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCost(serviceCostId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseMessage = Assert.IsType<Response<ServiceCostResponseDto>>(notFoundResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Service cost not found.", responseMessage.Message);
            Assert.Null(responseMessage.Data);
        }



        [Fact]
        public async Task CreateServiceCost_ReturnsOkResult_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            var response = new Response<ServiceCostResponseDto>(
                new ServiceCostResponseDto
                {
                    HostelId = request.HostelId,
                    ServiceId = request.ServiceId,
                    UnitCost = request.UnitCost,
                    EffectiveFrom = request.EffectiveFrom,
                    ServiceName = "Test Service"
                },
                "Service cost created successfully."
            );

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("Service cost created successfully.", returnValue.Message);
        }

        [Fact]
        public async Task CreateServiceCost_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseMessage = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Internal server error: Internal server error", responseMessage.Message);
        }


        [Fact]
        public async Task CreateServiceCost_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("HostelId", "Required");

            // Act
            var result = await _controller.CreateServiceCost(new CreateServiceCostDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateServiceCost_ReturnsBadRequest_WhenServiceNotFound()
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            var response = new Response<ServiceCostResponseDto>
            {
                Succeeded = false,
                Message = "Dịch vụ không tồn tại"
            };

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(badRequestResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Dịch vụ không tồn tại", returnValue.Message);
        }

        [Fact]
        public async Task CreateServiceCost_ReturnsBadRequest_WhenHostelNotFound()
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            var response = new Response<ServiceCostResponseDto>
            {
                Succeeded = false,
                Message = "Nhà trọ không tồn tại"
            };

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(badRequestResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Nhà trọ không tồn tại", returnValue.Message);
        }

        [Fact]
        public async Task UpdateServiceCost_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var updateDto = new UpdateServiceCostDto
            {
                UnitCost = 100,
                Unit = UnitType.Kwh,
                EffectiveFrom = DateTime.Now,
                EffectiveTo = null
            };

            var response = new Response<ServiceCostResponseDto>(
                new ServiceCostResponseDto
                {
                    UnitCost = 100,
                    Unit = UnitType.Kwh
                },
                "Service cost updated successfully."
            );

            _serviceCostServiceMock.Setup(s => s.UpdateAsync(serviceCostId, updateDto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateServiceCost(serviceCostId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("Service cost updated successfully.", returnValue.Message);
        }

        [Fact]
        public async Task UpdateServiceCost_ReturnsNotFound_WhenServiceCostDoesNotExist()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var updateDto = new UpdateServiceCostDto
            {
                UnitCost = 100,
                Unit = UnitType.Kwh,
                EffectiveFrom = DateTime.Now,
                EffectiveTo = null
            };

            var response = new Response<ServiceCostResponseDto>("Service cost not found.")
            {
                Succeeded = false
            };

            _serviceCostServiceMock.Setup(s => s.UpdateAsync(serviceCostId, updateDto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateServiceCost(serviceCostId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(notFoundResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Service cost not found.", returnValue.Message);
        }

        [Fact]
        public async Task UpdateServiceCost_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var updateDto = new UpdateServiceCostDto
            {
                UnitCost = -10, // Invalid UnitCost
                Unit = UnitType.Kwh,
                EffectiveFrom = DateTime.Now,
                EffectiveTo = null
            };

            _controller.ModelState.AddModelError("UnitCost", "UnitCost must be non-negative.");

            // Act
            var result = await _controller.UpdateServiceCost(serviceCostId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateServiceCost_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var updateDto = new UpdateServiceCostDto
            {
                UnitCost = 100,
                Unit = UnitType.Kwh,
                EffectiveFrom = DateTime.Now,
                EffectiveTo = null
            };

            _serviceCostServiceMock.Setup(s => s.UpdateAsync(serviceCostId, updateDto))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.UpdateServiceCost(serviceCostId, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error: Internal server error", objectResult.Value);
        }

        [Fact]
        public async Task DeleteServiceCost_ReturnsOkResult_WhenDeletionSucceeds()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var mockResponse = new Response<bool>
            {
                Data = true,
                Succeeded = true
            };

            _serviceCostServiceMock
                .Setup(service => service.DeleteAsync(serviceCostId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.DeleteServiceCost(serviceCostId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(returnValue.Data);
        }

        [Fact]
        public async Task DeleteServiceCost_ReturnsNotFound_WhenServiceCostDoesNotExist()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var mockResponse = new Response<bool>
            {
                Data = false,
                Succeeded = false,
                Message = "Service cost not found."
            };

            _serviceCostServiceMock
                .Setup(service => service.DeleteAsync(serviceCostId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.DeleteServiceCost(serviceCostId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Service cost not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteServiceCost_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.DeleteServiceCost(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("Invalid service cost ID", responseMessage);
        }

        [Fact]
        public async Task DeleteServiceCost_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();

            _serviceCostServiceMock.Setup(service => service.DeleteAsync(serviceCostId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteServiceCost(serviceCostId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseMessage = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Internal server error: Internal server error", responseMessage.Message);
        }

        [Fact]
        public async Task CreateServiceCost_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var invalidServiceCostDto = new CreateServiceCostDto();

            _controller.ModelState.AddModelError("ServiceName", "Required");

            // Act
            var result = await _controller.CreateServiceCost(invalidServiceCostDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseMessage = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Invalid model state.", responseMessage.Message);
        }

        [Theory]
        [InlineData(true, "Service cost created successfully", 200)]  // Valid Input
        [InlineData(false, "Nhà trọ không tồn tại", 400)]             // Hostel not found
        [InlineData(false, "Dịch vụ không tồn tại", 400)]             // Service not found
        [InlineData(false, "Đã tồn tại bảng giá dịch vụ cho dịch vụ này tại hostel vào thời điểm này.", 400)] // Duplicate service cost
        public async Task CreateServiceCost_WithDifferentInputs_ReturnsExpectedResult(bool success, string message, int expectedStatusCode)
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            var response = new Response<ServiceCostResponseDto>(
                success ? new ServiceCostResponseDto { HostelId = request.HostelId, ServiceId = request.ServiceId } : null,
                message
            )
            {
                Succeeded = success
            };

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            if (expectedStatusCode == 200)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(okResult.Value);
                Assert.True(returnValue.Succeeded);
                Assert.Equal(message, returnValue.Message);
            }
            else if (expectedStatusCode == 400)
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(badRequestResult.Value);
                Assert.False(returnValue.Succeeded);
                Assert.Equal(message, returnValue.Message);
            }
        }

        [Fact]
        public async Task CreateServiceCost_ReturnsBadRequest_WhenDuplicateServiceCostExists()
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            var response = new Response<ServiceCostResponseDto>
            {
                Succeeded = false,
                Message = "Đã tồn tại bảng giá dịch vụ cho dịch vụ này tại hostel vào thời điểm này."
            };

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<Response<ServiceCostResponseDto>>(badRequestResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Đã tồn tại bảng giá dịch vụ cho dịch vụ này tại hostel vào thời điểm này.", returnValue.Message);
        }

        [Fact]
        public async Task CreateServiceCost_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            var request = new CreateServiceCostDto
            {
                HostelId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                UnitCost = 1000,
                EffectiveFrom = DateTime.Now
            };

            _serviceCostServiceMock.Setup(service => service.CreateServiceCost(request))
                .ThrowsAsync(new KeyNotFoundException("Hostel not found."));

            // Act
            var result = await _controller.CreateServiceCost(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseMessage = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Hostel not found.", responseMessage.Message);
        }

        [Fact]
        public async Task GetServiceCosts_ReturnsOkResult_WhenServiceCostsExist()
        {
            // Arrange
            var mockServiceCosts = new List<ServiceCostResponseDto>
    {
        new ServiceCostResponseDto { Id = Guid.NewGuid(), ServiceName = "Electricity", UnitCost = 5 },
        new ServiceCostResponseDto { Id = Guid.NewGuid(), ServiceName = "Water", UnitCost = 3 }
    };

            var mockResponse = new Response<List<ServiceCostResponseDto>>(mockServiceCosts)
            {
                Succeeded = true,
                Message = "Service costs retrieved successfully"
            };

            _serviceCostServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCosts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<ServiceCostResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal(2, responseData.Data.Count);
            Assert.Equal("Service costs retrieved successfully", responseData.Message);
        }


        [Fact]
        public async Task GetServiceCosts_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var mockResponse = new Response<List<ServiceCostResponseDto>>(null, "Failed to retrieve service costs")
            {
                Succeeded = false
            };

            _serviceCostServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCosts();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseMessage = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Failed to retrieve service costs", responseMessage.Message);
        }


        [Fact]
        public async Task GetServiceCosts_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _serviceCostServiceMock.Setup(service => service.GetAllAsync())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetServiceCosts();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var response = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Internal server error", response.Message);
        }


        [Fact]
        public async Task GetServiceCosts_ReturnsEmptyList_WhenNoServiceCostsExist()
        {
            // Arrange
            var mockResponse = new Response<List<ServiceCostResponseDto>>(new List<ServiceCostResponseDto>(), "No service costs found")
            {
                Succeeded = true
            };

            _serviceCostServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCosts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<ServiceCostResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Empty(responseData.Data);
            Assert.Equal("No service costs found", responseData.Message);
        }

        [Fact]
        public async Task GetServiceCost_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.GetServiceCost(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseMessage = Assert.IsType<Response<ServiceCostResponseDto>>(badRequestResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Invalid ID", responseMessage.Message);
            Assert.Null(responseMessage.Data);
        }


        [Fact]
        public async Task GetServiceCost_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var id = Guid.NewGuid();

            _serviceCostServiceMock.Setup(service => service.GetByIdAsync(id))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetServiceCost(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var responseMessage = Assert.IsType<Response<ServiceCostResponseDto>>(objectResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Internal server error: Internal server error", responseMessage.Message);
            Assert.Null(responseMessage.Data);
        }



        [Fact]
        public async Task GetServiceCost_ReturnsValidData_WhenIdExists()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var mockServiceCost = new ServiceCostResponseDto
            {
                Id = serviceCostId,
                ServiceName = "Electricity",
                UnitCost = 100.0m
            };

            var mockResponse = new Response<ServiceCostResponseDto>(mockServiceCost);

            _serviceCostServiceMock.Setup(service => service.GetByIdAsync(serviceCostId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCost(serviceCostId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<ServiceCostResponseDto>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.NotNull(responseData.Data);
            Assert.Equal(serviceCostId, responseData.Data.Id);
            Assert.Equal("Electricity", responseData.Data.ServiceName);
            Assert.Equal(100.0m, responseData.Data.UnitCost);
        }

        [Fact]
        public async Task GetServiceCost_HandlesNullDataGracefully()
        {
            // Arrange
            var serviceCostId = Guid.NewGuid();
            var mockResponse = new Response<ServiceCostResponseDto>(null, "Service cost not found")
            {
                Succeeded = false
            };

            _serviceCostServiceMock.Setup(service => service.GetByIdAsync(serviceCostId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCost(serviceCostId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<ServiceCostResponseDto>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Service cost not found", responseData.Message);
            Assert.Null(responseData.Data);
        }

        [Fact]
        public async Task GetServiceCostsByHostel_ReturnsOkResult_WhenServiceCostsExist()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var mockServiceCosts = new List<ServiceCostResponseDto>
    {
        new ServiceCostResponseDto { Id = Guid.NewGuid(), HostelId = hostelId, ServiceName = "Electricity", UnitCost = 500 },
        new ServiceCostResponseDto { Id = Guid.NewGuid(), HostelId = hostelId, ServiceName = "Water", UnitCost = 300 }
    };

            var mockResponse = new Response<List<ServiceCostResponseDto>>(mockServiceCosts, "Lấy danh sách dịch vụ giá trọ thành công");

            _serviceCostServiceMock.Setup(service => service.GetAllServiceCostByHostel(hostelId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCostsByHostel(hostelId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<ServiceCostResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal(2, responseData.Data.Count);
            Assert.Equal("Lấy danh sách dịch vụ giá trọ thành công", responseData.Message);
        }

        [Fact]
        public async Task GetServiceCostsByHostel_ReturnsOkResultWithEmptyList_WhenNoServiceCostsExist()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var mockResponse = new Response<List<ServiceCostResponseDto>>(new List<ServiceCostResponseDto>(), "Không tìm thấy giá của dịch vụ trong nhà trọ");

            _serviceCostServiceMock.Setup(service => service.GetAllServiceCostByHostel(hostelId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCostsByHostel(hostelId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<ServiceCostResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Empty(responseData.Data);
            Assert.Equal("Không tìm thấy giá của dịch vụ trong nhà trọ", responseData.Message);
        }


        [Fact]
        public async Task GetServiceCostsByHostel_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var mockResponse = new Response<List<ServiceCostResponseDto>>(new List<ServiceCostResponseDto>(), "Failed to retrieve service costs")
            {
                Succeeded = false
            };

            _serviceCostServiceMock.Setup(service => service.GetAllServiceCostByHostel(hostelId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetServiceCostsByHostel(hostelId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsType<Response<List<ServiceCostResponseDto>>>(badRequestResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Failed to retrieve service costs", responseData.Message);
            Assert.Empty(responseData.Data); // Adjusted assertion to check for an empty list
        }



        [Fact]
        public async Task GetServiceCostsByHostel_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var hostelId = Guid.NewGuid();

            _serviceCostServiceMock.Setup(service => service.GetAllServiceCostByHostel(hostelId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetServiceCostsByHostel(hostelId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseMessage = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Equal("Internal server error: Internal server error", responseMessage.Message);
        }


    }
}
