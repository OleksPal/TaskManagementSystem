using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    [Collection("TestCollection")]
    public class UserTaskControllerTests
    {
        protected readonly UserTaskController _userTaskController;

        public UserTaskControllerTests()
        {
            var userManagerMock = Helper.MockUserManager<User>([Helper.ExistingUser]);
            var loggerMock = new Mock<ILogger<UserController>>();
            var userTaskService = Helper.GetRequiredService<IUserTaskService>()
                ?? throw new ArgumentNullException(nameof(IUserTaskService));
            
            var userTaskControllerMock = new Mock<UserTaskController>
                (userTaskService,userManagerMock.Object,loggerMock.Object);

            userTaskControllerMock.CallBase = true;

            userTaskControllerMock.Protected()
                .SetupSequence<Task<User>>("GetUser")
                .Returns(Task.FromResult(Helper.ExistingUser))
                .Returns(Task.FromResult(new User { Id = Guid.Empty }));

            _userTaskController = userTaskControllerMock.Object;
        }

        #region GetAllTasksAsync
        [Fact]
        public async Task GetAllTasksAsync_ByExistingUser_ReturnsNotEmpty()
        {
            // Arrange
            var query = new QueryObject();

            // Act
            var actionResult = await _userTaskController.GetAllTasksAsync(query);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var taskDtoList = okResult.Value as ICollection<UserTaskDto>;
            Assert.NotEmpty(taskDtoList);
        }

        [Fact]
        public async Task GetAllTasksAsync_ByNonExistentUser_ReturnsEmpty()
        {
            // Arrange
            var query = new QueryObject();

            // Act
            await _userTaskController.GetAllTasksAsync(query);
            // Calling a method a second time so that the method is called by a non-existent user
            var actionResult = await _userTaskController.GetAllTasksAsync(query); 

            // Assert
            var okResult = actionResult as OkObjectResult;
            var taskDtoList = okResult.Value as ICollection<UserTaskDto>;
            Assert.Empty(taskDtoList);
        }
        #endregion
    }
}
