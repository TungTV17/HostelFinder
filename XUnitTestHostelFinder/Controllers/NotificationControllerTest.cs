using Moq;
using HostelFinder.Application.DTOs.Notification;
using Microsoft.AspNetCore.Mvc;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.WebApi.Controllers;

namespace XUnitTestHostelFinder.Controllers
{
    public class NotificationControllerTest
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly NotificationController _controller;

        public NotificationControllerTest()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _controller = new NotificationController(_notificationServiceMock.Object);
        }

        // Test Case 2: Notifications Found
        [Fact]
        public async Task GetMessagesByUserId_ShouldReturnOk_WhenNotificationsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            
            // Mock the service to return a list of notifications
            var notifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Message = "New message", TimeAgo = "2 hours ago" },
                new NotificationResponseDto { Message = "Account updated", TimeAgo = "1 day ago" }
            };

            _notificationServiceMock.Setup(service => service.GetMessagesByUserIdAsync(userId))
                                     .ReturnsAsync(notifications);

            // Act
            var result = await _controller.GetMessagesByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<List<NotificationResponseDto>>(okResult.Value);
            Assert.Equal(2, response.Count);  // Ensure there are 2 notifications returned
        }

    }
}
