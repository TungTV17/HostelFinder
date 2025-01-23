using HostelFinder.Application.DTOs.Room;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
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
    public class RoomControllerTests
    {
        private readonly Mock<IRoomService> _roomServiceMock;
        private readonly Mock<ITenantService> _tenantServiceMock;
        private readonly Mock<IRoomTenancyService> _roomTenacyServiceMock;
        private readonly RoomController _controller;

        public RoomControllerTests()
        {
            _tenantServiceMock = new Mock<ITenantService>();
            _roomTenacyServiceMock = new Mock<IRoomTenancyService>();
            _roomServiceMock = new Mock<IRoomService>();
            _controller = new RoomController(_roomServiceMock.Object, _tenantServiceMock.Object, _roomTenacyServiceMock.Object);

        }

        [Fact]
        public async Task GetRooms_ShouldReturnOk_WhenRoomsFound()
        {
            // Arrange
            var roomResponseDtos = new List<RoomResponseDto>
    {
        new RoomResponseDto { Id = Guid.NewGuid(), RoomName = "Room 1" },
        new RoomResponseDto { Id = Guid.NewGuid(), RoomName = "Room 2" }
    };

            _roomServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new Response<List<RoomResponseDto>>(roomResponseDtos));

            // Act
            var result = await _controller.GetRooms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<RoomResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(2, response.Data.Count);
            Assert.Equal("Room 1", response.Data[0].RoomName);
            Assert.Equal("Room 2", response.Data[1].RoomName);
        }

        [Fact]
        public async Task GetRooms_ShouldReturnOk_WhenNoRoomsFound()
        {
            // Arrange
            _roomServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new Response<List<RoomResponseDto>>(new List<RoomResponseDto>(), "No rooms found"));

            // Act
            var result = await _controller.GetRooms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<RoomResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Empty(response.Data);  // Ensure the data list is empty
            Assert.Equal("No rooms found", response.Message);
        }

        [Fact]
        public async Task GetRooms_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _roomServiceMock.Setup(service => service.GetAllAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetRooms();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: Unexpected error", response.Message);
        }

        [Fact]
        public async Task GetRoom_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            _roomServiceMock.Setup(service => service.GetByIdAsync(roomId))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetRoom(roomId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error: Something went wrong", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetRoom_ShouldReturnOk_WhenRoomIsFound()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var roomDto = new RoomByIdDto
            {
                Id = roomId,
                RoomName = "Test Room",
                ImageRoom = new List<string> { "url1", "url2" }
            };

            _roomServiceMock.Setup(service => service.GetByIdAsync(roomId))
                .ReturnsAsync(new Response<RoomByIdDto>(roomDto));

            // Act
            var result = await _controller.GetRoom(roomId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<RoomByIdDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(roomDto, response.Data);
        }

        [Fact]
        public async Task CreateRoom_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var roomDto = new AddRoomRequestDto
            {
                HostelId = Guid.NewGuid(),
                RoomName = "Room 101",
                Floor = 1,
                MaxRenters = 4,
                Deposit = 500m,
                MonthlyRentCost = 1500m,
                Size = 20.5m,
                RoomType = RoomType.Phòng_trọ,
                AmenityId = new List<Guid> { Guid.NewGuid() }
            };

            var roomImages = new List<IFormFile>(); // Mock image files

            _roomServiceMock.Setup(service => service.CreateRoomAsync(roomDto, roomImages))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.CreateRoom(roomDto, roomImages);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Internal server error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task CreateRoom_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var roomDto = new AddRoomRequestDto
            {
                HostelId = Guid.NewGuid(),
                RoomName = "", // Invalid room name
                Floor = 1,
                MaxRenters = 4,
                Deposit = 500m,
                MonthlyRentCost = 1500m,
                Size = 20.5m,
                RoomType = RoomType.Phòng_trọ,
                AmenityId = new List<Guid> { Guid.NewGuid() }
            };

            var roomImages = new List<IFormFile>(); // Mock image files

            // Simulate ModelState being invalid
            _controller.ModelState.AddModelError("RoomName", "Room name is required.");

            // Act
            var result = await _controller.CreateRoom(roomDto, roomImages);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("RoomName"));
        }

        [Fact]
        public async Task CreateRoom_ShouldReturnBadRequest_WhenRoomNameExists()
        {
            // Arrange
            var roomDto = new AddRoomRequestDto
            {
                HostelId = Guid.NewGuid(),
                RoomName = "Room 101",
                Floor = 1,
                MaxRenters = 4,
                Deposit = 500m,
                MonthlyRentCost = 1500m,
                Size = 20.5m,
                RoomType = RoomType.Phòng_trọ,
                AmenityId = new List<Guid> { Guid.NewGuid() }
            };

            var roomImages = new List<IFormFile>(); // Mock image files

            _roomServiceMock.Setup(service => service.CreateRoomAsync(roomDto, roomImages))
                .ReturnsAsync(new Response<RoomResponseDto>("Tên phòng đã tồn tại trong trọ."));

            // Act
            var result = await _controller.CreateRoom(roomDto, roomImages);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<RoomResponseDto>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Tên phòng đã tồn tại trong trọ.", response.Message);
        }

        [Fact]
        public async Task CreateRoom_ShouldReturnOk_WhenRoomIsCreatedSuccessfully()
        {
            // Arrange
            var roomDto = new AddRoomRequestDto
            {
                HostelId = Guid.NewGuid(),
                RoomName = "Room 101",
                Floor = 1,
                MaxRenters = 4,
                Deposit = 500m,
                MonthlyRentCost = 1500m,
                Size = 20.5m,
                RoomType = RoomType.Chung_cư_mini,
                AmenityId = new List<Guid> { Guid.NewGuid() }
            };

            var roomImages = new List<IFormFile>(); // Add some mock image files

            _roomServiceMock.Setup(service => service.CreateRoomAsync(roomDto, roomImages))
                .ReturnsAsync(new Response<RoomResponseDto>
                {
                    Succeeded = true,
                    Message = "Thêm phòng trọ thành công"
                });

            // Act
            var result = await _controller.CreateRoom(roomDto, roomImages);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<RoomResponseDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Thêm phòng trọ thành công", response.Message);
        }

        [Fact]
        public async Task DeleteRoom_ShouldReturnBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.DeleteRoom(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid room ID", response.Message);
        }

        [Fact]
        public async Task DeleteRoom_ShouldReturnOk_WhenRoomIsDeletedSuccessfully()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var roomServiceMock = new Mock<IRoomService>();
            roomServiceMock.Setup(service => service.DeleteAsync(roomId))
                .ReturnsAsync(new Response<bool>(true, "Xóa phòng thành công.")); // Simulate the service returning a successful deletion response

            var controller = new RoomController(roomServiceMock.Object, _tenantServiceMock.Object, _roomTenacyServiceMock.Object);

            // Act
            var result = await controller.DeleteRoom(roomId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<bool>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Xóa phòng thành công.", response.Message); // Ensure the success message is correct
        }

        [Fact]
        public async Task GetRoomsByHostelId_ShouldReturnMessage_WhenNoRoomsFound()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var floor = 1;

            // Simulate no rooms available for the given hostelId and floor
            var roomServiceMock = new Mock<IRoomService>();
            roomServiceMock.Setup(service => service.GetRoomsByHostelIdAsync(hostelId, floor))
                .ReturnsAsync(new Response<List<RoomResponseDto>>(new List<RoomResponseDto>(), "Hiện tại chưa có phòng trọ nào"));

            var imageRepositoryMock = new Mock<IImageRepository>(); // Not used in this case, but mocked for consistency
            var controller = new RoomController(roomServiceMock.Object, _tenantServiceMock.Object, _roomTenacyServiceMock.Object);

            // Act
            var result = await controller.GetRoomsByHostelId(hostelId, floor);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<RoomResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Hiện tại chưa có phòng trọ nào", response.Message);
        }


        [Fact]
        public async Task GetRoomsByHostelId_ShouldReturnOk_WhenRoomsExist()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var floor = 1;

            // Simulate the rooms returned by the service
            var roomDtos = new List<RoomResponseDto>
    {
        new RoomResponseDto { Id = Guid.NewGuid(), RoomName = "Room 101", Floor = 1 },
        new RoomResponseDto { Id = Guid.NewGuid(), RoomName = "Room 102", Floor = 1 }
    };

            // Simulate the service call
            var roomServiceMock = new Mock<IRoomService>();
            roomServiceMock.Setup(service => service.GetRoomsByHostelIdAsync(hostelId, floor))
                .ReturnsAsync(new Response<List<RoomResponseDto>>(roomDtos));

            // Simulate the image URL repository
            var imageRepositoryMock = new Mock<IImageRepository>();
            imageRepositoryMock.Setup(repo => repo.GetImageUrlByRoomId(It.IsAny<Guid>()))
                .ReturnsAsync(new Image { Url = "https://hostel-finder-images.s3.amazonaws.com/default-room.jpg" });

            var controller = new RoomController(roomServiceMock.Object, _tenantServiceMock.Object, _roomTenacyServiceMock.Object);

            // Act
            var result = await controller.GetRoomsByHostelId(hostelId, floor);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<RoomResponseDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(2, response.Data.Count);  // We expect 2 rooms in the response
        }

        [Fact]
        public async Task GetRoomsByHostelId_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var hostelId = Guid.NewGuid();
            var errorResponse = new Response<List<SelectRoomResponse>>
            {
                Succeeded = false,
                Message = "Error retrieving rooms."
            };

            _roomServiceMock.Setup(service => service.GetSelectRoomByHostelAsync(hostelId))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.GetRoomsByHostelId(hostelId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<List<SelectRoomResponse>>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Error retrieving rooms.", response.Message);
        }

    }
}
