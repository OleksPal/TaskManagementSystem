﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
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
                (userTaskService, userManagerMock.Object, loggerMock.Object);

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

        #region GetTaskAsync
        [Fact]
        public async Task GetTaskAsync_TaskDoesNotExists_ByExistingUser_ReturnsNotFound()
        {
            // Arrange
            var nonExistentTaskId = Guid.Empty;

            // Act
            var actionResult = await _userTaskController.GetTaskAsync(nonExistentTaskId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task GetTaskAsync_TaskDoesNotExists_ByNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var nonExistentTaskId = Guid.Empty;

            // Act
            await _userTaskController.GetTaskAsync(nonExistentTaskId);
            // Calling a method a second time so that the method is called by a non-existent user
            var actionResult = await _userTaskController.GetTaskAsync(nonExistentTaskId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task GetTaskAsync_TaskThatExists_ByExistingUser_ReturnsNotNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;

            // Act
            var actionResult = await _userTaskController.GetTaskAsync(existingTaskId);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var taskDto = okResult.Value as UserTaskDto;
            Assert.NotNull(taskDto);
        }

        [Fact]
        public async Task GetTaskAsync_TaskThatExists_ByNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;

            // Act
            await _userTaskController.GetTaskAsync(existingTaskId);
            // Calling a method a second time so that the method is called by a non-existent user
            var actionResult = await _userTaskController.GetTaskAsync(existingTaskId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }
        #endregion

        #region CreateTaskAsync
        [Fact]
        public async Task CreteTaskAsync_Null_ReturnsNullReferenceException()
        {
            // Arrange
            CreateUserTaskRequestDto createTaskDto = null;

            // Act
            Func<Task> act = () => _userTaskController.CreateTaskAsync(createTaskDto);

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task CreateTaskAsync_InvalidTaskWithoutRequiredProperties_ReturnsDbUpdateException()
        {
            // Arrange
            CreateUserTaskRequestDto createTaskDto = new();

            // Act
            Func<Task> act = () => _userTaskController.CreateTaskAsync(createTaskDto);

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task AddTaskAsync_ValidTask_ReturnsNotNull()
        {
            // Arrange
            var createTaskDto = new CreateUserTaskRequestDto
            {
                Title = "AddValidTask",
                Status = Status.Completed,
                Priority = Priority.Low,
                UserId = Helper.ExistingUser.Id
            };
            var existingUserId = Helper.ExistingUser.Id;

            //Act
            var actionResult = await _userTaskController.CreateTaskAsync(createTaskDto);

            // Assert
            var okResult = actionResult as ObjectResult;
            var taskDto = okResult.Value as UserTaskDto;
            Assert.NotNull(taskDto);
        }
        #endregion
    }
}
