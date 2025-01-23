using HostelFinder.Application.DTOs.Invoice.Responses;
using HostelFinder.Application.DTOs.InVoice.Requests;
using HostelFinder.Application.DTOs.InVoice.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestHostelFinder.Controllers
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IInvoiceService> _invoiceServiceMock;
        private readonly InvoiceController _controller;

        public InvoiceControllerTests()
        {
            _invoiceServiceMock = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_invoiceServiceMock.Object);
        }

        [Fact]
        public async Task GetInvoices_ReturnsOkResult_WhenServiceSucceeds()
        {
            // Arrange
            var invoices = new List<InvoiceResponseDto>
                    {
                        new InvoiceResponseDto
                        {
                            RoomName = "Room 101",
                            BillingMonth = 10,
                            BillingYear = 2024,
                            TotalAmount = 500,
                            IsPaid = true
                        }
                    };

            var mockResponse = new Response<List<InvoiceResponseDto>>(invoices, "Invoices retrieved successfully");

            _invoiceServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetInvoices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<InvoiceResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal(1, responseData.Data.Count);
        }


        [Fact]
        public async Task GetInvoices_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var mockResponse = new Response<List<InvoiceResponseDto>>(null, "Service failed")
            {
                Succeeded = false
            };

            _invoiceServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetInvoices();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsType<Response<List<InvoiceResponseDto>>>(badRequestResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Service failed", responseData.Message);
        }

        [Fact]
        public async Task GetInvoices_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _invoiceServiceMock.Setup(service => service.GetAllAsync())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetInvoices();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var responseData = Assert.IsType<Response<List<InvoiceResponseDto>>>(objectResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Internal server error: Internal server error", responseData.Message);
        }

        [Fact]
        public async Task GetInvoices_ReturnsEmptyList_WhenNoInvoicesExist()
        {
            // Arrange
            var mockResponse = new Response<List<InvoiceResponseDto>>(new List<InvoiceResponseDto>(), "No invoices found")
            {
                Succeeded = true
            };

            _invoiceServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetInvoices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<List<InvoiceResponseDto>>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Empty(responseData.Data);
            Assert.Equal("No invoices found", responseData.Message);
        }

        [Fact]
        public async Task GetInvoice_ReturnsOkResult_WhenInvoiceExists()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var mockInvoice = new InvoiceResponseDto { RoomName = "Room A", TotalAmount = 150 };
            var response = new Response<InvoiceResponseDto>(mockInvoice);

            _invoiceServiceMock.Setup(service => service.GetByIdAsync(invoiceId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetInvoice(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<Response<InvoiceResponseDto>>(okResult.Value);
            Assert.True(responseData.Succeeded);
            Assert.Equal("Room A", responseData.Data.RoomName);
            Assert.Equal(150, responseData.Data.TotalAmount);
        }

        [Fact]
        public async Task GetInvoice_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var mockResponse = new Response<InvoiceResponseDto>
            {
                Succeeded = false,
                Message = "Invoice not found"
            };

            _invoiceServiceMock.Setup(service => service.GetByIdAsync(invoiceId))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetInvoice(invoiceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseData = Assert.IsType<Response<InvoiceResponseDto>>(notFoundResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Invoice not found", responseData.Message);
        }

        [Fact]
        public async Task GetInvoice_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.GetInvoice(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsType<Response<InvoiceResponseDto>>(badRequestResult.Value);
            Assert.False(responseData.Succeeded);
            Assert.Equal("Invalid ID", responseData.Message);
        }

        [Fact]
        public async Task CreateInvoice_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var invoiceDto = new AddInVoiceRequestDto { roomId = Guid.NewGuid(), billingMonth = 11, billingYear = 2024 };
            var invoiceResponse = new Response<InvoiceResponseDto>(
                new InvoiceResponseDto
                {
                    RoomName = "Room 101",
                    BillingMonth = 11,
                    BillingYear = 2024,
                    TotalAmount = 1000,
                    IsPaid = false,
                    InvoiceDetails = new List<InvoiceDetailResponseDto>()
                },
                "Invoice created successfully"
            );

            _invoiceServiceMock.Setup(service => service.GenerateMonthlyInvoicesAsync(invoiceDto.roomId, invoiceDto.billingMonth, invoiceDto.billingYear))
                .ReturnsAsync(invoiceResponse);

            // Act
            var result = await _controller.CreateInvoice(invoiceDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            var returnValue = Assert.IsType<Response<InvoiceResponseDto>>(objectResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("Invoice created successfully", returnValue.Message);
            Assert.NotNull(returnValue.Data);
            Assert.Equal(11, returnValue.Data.BillingMonth);
            Assert.Equal(2024, returnValue.Data.BillingYear);
        }

        [Fact]
        public async Task CreateInvoice_ReturnsBadRequest_WhenInvalidModelState()
        {
            // Arrange
            _controller.ModelState.AddModelError("roomId", "Required");

            // Act
            var result = await _controller.CreateInvoice(new AddInVoiceRequestDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var updateDto = new UpdateInvoiceRequestDto { TotalAmount = 150 };
            var response = new Response<InvoiceResponseDto>(new InvoiceResponseDto { TotalAmount = 150 }, "Invoice updated successfully");

            _invoiceServiceMock.Setup(service => service.UpdateAsync(invoiceId, updateDto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateInvoice(invoiceId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<InvoiceResponseDto>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal(150, returnValue.Data.TotalAmount);
            Assert.Equal("Invoice updated successfully", returnValue.Message);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var invoiceDto = new UpdateInvoiceRequestDto { TotalAmount = 1000 };

            _invoiceServiceMock.Setup(service => service.UpdateAsync(invoiceId, invoiceDto))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.UpdateInvoice(invoiceId, invoiceDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<string>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Internal server error", response.Message);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.UpdateInvoice(invalidId, new UpdateInvoiceRequestDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseMessage = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(responseMessage.Succeeded);
            Assert.Contains("Invalid invoice ID", responseMessage.Message);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var updateDto = new UpdateInvoiceRequestDto { TotalAmount = 150 };
            var response = new Response<InvoiceResponseDto>
            {
                Succeeded = false,
                Message = "Invoice not found"
            };

            _invoiceServiceMock.Setup(service => service.UpdateAsync(invoiceId, updateDto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateInvoice(invoiceId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<Response<string>>(notFoundResult.Value);
            Assert.False(returnValue.Succeeded);
            Assert.Equal("Invoice not found", returnValue.Message);
        }

        // Test for DeleteInvoice
        [Fact]
        public async Task DeleteInvoice_ReturnsOkResult_WhenInvoiceIsDeleted()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var response = new Response<bool>(true, "Invoice deleted successfully");

            _invoiceServiceMock.Setup(service => service.DeleteAsync(invoiceId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteInvoice(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(returnValue.Succeeded);
        }

        [Theory]
        [InlineData(true, "Invoice created successfully", 200)]
        [InlineData(false, "Invoice creation failed", 400)]
        public async Task CreateInvoice_ReturnsExpectedResult(bool success, string message, int expectedStatusCode)
        {
            // Arrange
            var invoiceDto = new AddInVoiceRequestDto { roomId = Guid.NewGuid(), billingMonth = 11, billingYear = 2024 };
            var response = new Response<InvoiceResponseDto>(success ? new InvoiceResponseDto() : null, message)
            {
                Succeeded = success
            };

            _invoiceServiceMock.Setup(service => service.GenerateMonthlyInvoicesAsync(invoiceDto.roomId, invoiceDto.billingMonth, invoiceDto.billingYear))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateInvoice(invoiceDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedStatusCode, objectResult.StatusCode);
            var returnValue = Assert.IsType<Response<InvoiceResponseDto>>(objectResult.Value);
            Assert.Equal(success, returnValue.Succeeded);
            Assert.Equal(message, returnValue.Message);
        }

        [Fact]
        public async Task GetInvoice_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            _invoiceServiceMock.Setup(service => service.GetByIdAsync(invoiceId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetInvoice(invoiceId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<InvoiceResponseDto>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Internal server error", response.Message);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var updateDto = new UpdateInvoiceRequestDto();
            _controller.ModelState.AddModelError("TotalAmount", "Required");

            // Act
            var result = await _controller.UpdateInvoice(invoiceId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteInvoice_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            _invoiceServiceMock.Setup(service => service.DeleteAsync(invoiceId))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteInvoice(invoiceId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<Response<bool>>(objectResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Internal server error", response.Message);
        }

    }
}
