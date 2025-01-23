using HostelFinder.Application.DTOs.Membership.Requests;
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.DTOs.MembershipService.Requests;
using HostelFinder.Application.DTOs.MembershipService.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Services;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HostelFinder.UnitTests.Controllers
{
    public class MembershipControllerTests
    {
        private readonly MembershipController _controller;
        private readonly Mock<IMembershipService> _membershipServiceMock;
        private readonly Mock<IWalletService> _walletServiceMock;

        public MembershipControllerTests()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _walletServiceMock = new Mock<IWalletService>();
            _controller = new MembershipController(_membershipServiceMock.Object, _walletServiceMock.Object);
        }

        [Fact]
        public async Task GetListMembership_ShouldReturnOk_WhenMembershipsAreFound()
        {
            // Arrange
            var membershipList = new List<MembershipResponseDto>
            {
                new MembershipResponseDto
                {
                    /* populate properties */
                },
                new MembershipResponseDto
                {
                    /* populate properties */
                }
            };

            _membershipServiceMock.Setup(service => service.GetAllMembershipWithMembershipService())
                .ReturnsAsync(new Response<List<MembershipResponseDto>>
                {
                    Succeeded = true,
                    Data = membershipList
                });

            // Act
            var result = await _controller.GetListMembership();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<MembershipResponseDto>>>(actionResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(2, response.Data.Count); // Check if we have two memberships in the response
        }

        [Fact]
        public async Task GetListMembership_ShouldReturnNotFound_WhenNoMembershipsFound()
        {
            // Arrange
            _membershipServiceMock.Setup(service => service.GetAllMembershipWithMembershipService())
                .ReturnsAsync(new Response<List<MembershipResponseDto>>
                {
                    Succeeded = false,
                    Errors = new List<string> { "No memberships found." }
                });

            // Act
            var result = await _controller.GetListMembership();

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<List<MembershipResponseDto>>>(actionResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("No memberships found.", response.Errors);
        }

        [Fact]
        public async Task AddMembership_ShouldReturnOk_WhenMembershipIsCreated()
        {
            // Arrange
            var membershipDto = new AddMembershipRequestDto
            {
                // Set properties for AddMembershipRequestDto
            };
            var membershipResponseDto = new MembershipResponseDto
            {
                // Set properties for MembershipResponseDto
            };

            var response = new Response<MembershipResponseDto>
            {
                Data = membershipResponseDto,
                Message = "Gói thành viện đã tạo thành công!"
            };

            // Setup mock service call
            _membershipServiceMock.Setup(service => service.AddMembershipAsync(membershipDto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AddMembership(membershipDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnResponse = Assert.IsType<Response<MembershipResponseDto>>(okResult.Value);
            Assert.True(returnResponse.Succeeded);
            Assert.Equal("Gói thành viện đã tạo thành công!", returnResponse.Message);
        }

        // Test: Invalid ModelState
        [Fact]
        public async Task AddMembership_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var membershipDto = new AddMembershipRequestDto(); // Model is invalid
            _controller.ModelState.AddModelError("Error", "Model is invalid");

            // Act
            var result = await _controller.AddMembership(membershipDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value); // ModelState errors
        }

        [Fact]
        public async Task AddMembership_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var membershipDto = new AddMembershipRequestDto
            {
                // Set properties for AddMembershipRequestDto
            };

            // Setup mock service to throw an exception
            _membershipServiceMock.Setup(service => service.AddMembershipAsync(membershipDto))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.AddMembership(membershipDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // Expect ObjectResult
            Assert.Equal(500, objectResult.StatusCode); // Check for HTTP 500 status
            var response =
                Assert.IsType<Response<string>>(objectResult.Value); // Check if response contains error message
            Assert.Equal("An unexpected error occurred: Unexpected error", response.Message); // Check error message
        }

        [Fact]
        public async Task EditMembership_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var membershipDto = new UpdateMembershipRequestDto
            {
                Name = "Faulty Membership",
                Description = "This will throw an error",
                Price = 49.99m,
                Duration = 6
            };

            var id = Guid.NewGuid(); // Assume this is a valid ID

            _membershipServiceMock
                .Setup(service => service.EditMembershipAsync(id, It.IsAny<UpdateMembershipRequestDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.EditMembership(id, membershipDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.Contains("Unexpected error", response.Message);
        }

        [Fact]
        public async Task EditMembership_ShouldReturnNotFound_WhenMembershipDoesNotExist()
        {
            // Arrange
            var membershipDto = new UpdateMembershipRequestDto
            {
                Name = "Non-Existent Membership",
                Description = "Some description",
                Price = 49.99m,
                Duration = 6
            };

            var id = Guid.NewGuid(); // Assume this is a non-existing ID
            _membershipServiceMock
                .Setup(service => service.EditMembershipAsync(id, It.IsAny<UpdateMembershipRequestDto>()))
                .ReturnsAsync(new Response<MembershipResponseDto>("Membership not found."));

            // Act
            var result = await _controller.EditMembership(id, membershipDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<Response<MembershipResponseDto>>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Membership not found.", response.Message);
        }

        //[Fact]
        //public async Task EditMembership_ShouldReturnOk_WhenMembershipIsUpdatedSuccessfully()
        //{
        //    // Arrange
        //    var membershipDto = new UpdateMembershipRequestDto
        //    {
        //        Name = "Updated Membership",
        //        Description = "Updated Description",
        //        Price = 99.99m,
        //        Duration = 12,
        //        MembershipServices = new List<UpdateMembershipServiceReqDto>
        //        {
        //            new UpdateMembershipServiceReqDto
        //            {
        //                ServiceName = "Service 1",
        //                MaxPushTopAllowed = 5,
        //                MaxPostsAllowed = 10
        //            }
        //        }
        //    };

        //    var id = Guid.NewGuid(); // Assume this is a valid membership ID
        //    var responseDto = new MembershipResponseDto
        //    {
        //        Id = id,
        //        Name = "Updated Membership",
        //        Description = "Updated Description",
        //        Price = 99.99m,
        //        Duration = 12,
        //        MembershipServices = new MembershipServiceResponseDto
        //        {
        //            new MembershipServiceResponseDto
        //            {
        //                ServiceName = "Service 1",
        //                MaxPushTopAllowed = 5,
        //                MaxPostsAllowed = 10
        //            }
        //        }
        //    };

        //    _membershipServiceMock
        //        .Setup(service => service.EditMembershipAsync(id, It.IsAny<UpdateMembershipRequestDto>()))
        //        .ReturnsAsync(new Response<MembershipResponseDto>
        //        {
        //            Succeeded = true,
        //            Data = responseDto
        //        });

        //    // Act
        //    var result = await _controller.EditMembership(id, membershipDto);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var response = Assert.IsType<Response<MembershipResponseDto>>(okResult.Value);
        //    Assert.Equal(200, okResult.StatusCode);
        //    Assert.Equal("Updated Membership", response.Data.Name);
        //}

        [Fact]
        public async Task DeleteMembership_ShouldReturnBadRequest_WhenIdIsEmpty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act
            var result = await _controller.DeleteMembership(emptyId);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(actionResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid membership ID.", response.Message);
        }

        [Fact]
        public async Task DeleteMembership_ShouldReturnOk_WhenSuccessfullyDeleted()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            _membershipServiceMock.Setup(s => s.DeleteMembershipAsync(membershipId))
                .ReturnsAsync(new Response<bool>(true, "Membership deleted successfully."));

            // Act
            var result = await _controller.DeleteMembership(membershipId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<bool>>(actionResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal("Membership deleted successfully.", response.Message);
        }

        [Fact]
        public async Task DeleteMembership_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            _membershipServiceMock.Setup(s => s.DeleteMembershipAsync(membershipId))
                .ThrowsAsync(new Exception("An unexpected error occurred"));

            // Act
            var result = await _controller.DeleteMembership(membershipId);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<Response<string>>(actionResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Internal server error: An unexpected error occurred", response.Message);
        }

        [Fact]
        public async Task GetMembershipServicesForUser_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Mock the service method to throw an exception
            _membershipServiceMock.Setup(m => m.GetMembershipServicesForUserAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetMembershipServicesForUser(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<Response<string>>(statusCodeResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Internal server error", response.Message);
        }

        [Fact]
        public async Task GetMembershipServicesForUser_ShouldReturnBadRequest_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var result = await _controller.GetMembershipServicesForUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Succeeded);
            Assert.Equal("Invalid user ID.", response.Message);
        }

        [Fact]
        public async Task GetMembershipServicesForUser_ShouldReturnNotFound_WhenNoServicesFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Mock the service method to return no membership services
            _membershipServiceMock.Setup(m => m.GetMembershipServicesForUserAsync(userId))
                .ReturnsAsync(new Response<List<PostingMemberShipServiceDto>>
                {
                    Succeeded = false,
                    Message = "No membership services found for this user."
                });

            // Act
            var result = await _controller.GetMembershipServicesForUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); // Checking for NotFoundObjectResult
            var response =
                Assert.IsType<Response<List<PostingMemberShipServiceDto>>>(notFoundResult
                    .Value); // Correct type to assert
            Assert.False(response.Succeeded);
            Assert.Equal("No membership services found for this user.", response.Message);
        }
    }
}