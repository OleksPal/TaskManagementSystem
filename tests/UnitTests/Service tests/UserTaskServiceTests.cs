using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    [Collection("TestCollection")]
    public class UserTaskServiceTests
    {
        protected readonly IUserTaskService _userTaskService;

        public UserTaskServiceTests()
        {
            _userTaskService = Helper.GetRequiredService<IUserTaskService>()
                ?? throw new ArgumentNullException(nameof(IUserTaskService));
        }

        #region GetAllTasksAsync
        [Fact]
        public async Task GetAllTasksAsync_ByExistingUser_ReturnsNotEmpty()
        {
            // Arrange
            var query = new QueryObject();
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            var taskList = await _userTaskService.GetAllTasksAsync(existingUserId, query);

            // Assert
            Assert.NotEmpty(taskList);
        }

        [Fact]
        public async Task GetAllTasksAsync_ByNonExistentUser_ReturnsEmpty()
        {
            // Arrange
            var query = new QueryObject();
            var existingUserId = Guid.Empty;

            // Act
            var taskList = await _userTaskService.GetAllTasksAsync(existingUserId, query);

            // Assert
            Assert.Empty(taskList);
        }
        #endregion

        #region GetTaskByIdAsync
        [Theory]
        [MemberData(nameof(Helper.UserIds), MemberType = typeof(Helper))]
        public async Task GetTaskByIdAsync_TaskDoesNotExists_ReturnsNull(Guid userId)
        {
            // Arrange
            var nonExistentTaskId = Guid.Empty;

            // Act
            var task = await _userTaskService.GetTaskByIdAsync(nonExistentTaskId, userId);

            // Assert
            Assert.Null(task);
        }

        [Fact]
        public async Task GetTaskByIdAsync_TaskThatExists_ByExistingUser_ReturnsNotNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            var task = await _userTaskService.GetTaskByIdAsync(existingTaskId, existingUserId);

            // Assert
            Assert.NotNull(task);
        }

        [Fact]
        public async Task GetTaskByIdAsync_TaskThatExists_ByNonExistentUser_ReturnsNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var nonExistentUserId = Guid.Empty;

            // Act
            var task = await _userTaskService.GetTaskByIdAsync(existingTaskId, nonExistentUserId);

            // Assert
            Assert.Null(task);
        }
        #endregion

        #region AddTaskAsync
        [Fact]
        public async Task AddTaskAsync_Null_ReturnsNullReferenceException()
        {
            // Arrange
            CreateUserTaskRequestDto createTaskDto = null;

            // Act
            Func<Task> act = () => _userTaskService.AddTaskAsync(createTaskDto);

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task AddTaskAsync_InvalidTaskWithoutRequiredProperties_ReturnsDbUpdateException()
        {
            // Arrange
            CreateUserTaskRequestDto createTaskDto = new();

            // Act
            Func<Task> act = () => _userTaskService.AddTaskAsync(createTaskDto);

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
            var addedTask = await _userTaskService.AddTaskAsync(createTaskDto);

            // Assert
            var task = await _userTaskService.GetTaskByIdAsync(addedTask.Id, existingUserId);
            Assert.NotNull(task);
        }
        #endregion

        #region EditTaskAsync
        [Fact]
        public async Task EditTaskAsync_ExistingTaskWithNull_ReturnsNullReferenceException()
        {
            // Arrange
            UpdateUserTaskRequestDto updateTaskDto = null;
            var existingUserId = Helper.ExistingUser.Id;
            var existingTaskId = Helper.ExistingTask.Id;

            // Act
            Func<Task> act = () => _userTaskService.EditTaskAsync(existingTaskId, updateTaskDto, existingUserId);

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Theory]
        [MemberData(nameof(Helper.UserIds), MemberType = typeof(Helper))]
        public async Task EditTaskAsync_TaskThatDoesNotExists_ReturnsNull(Guid userId)
        {
            // Arrange
            var updateTaskDto = new UpdateUserTaskRequestDto
            {
                Title = "EditNonExistentTask",
                Status = Status.Completed,
                Priority = Priority.Low,
                UserId = Helper.ExistingUser.Id
            };
            var nonExistentTaskId = Guid.Empty;

            // Act
            var taskDto = await _userTaskService.EditTaskAsync(nonExistentTaskId, updateTaskDto, userId);

            // Assert
            Assert.Null(taskDto);
        }

        [Fact]
        public async Task EditTaskAsync_ValidTask_ByExistingUser_ReturnsUpdatedTask()
        {
            // Arrange
            var taskToUpdate = new UpdateUserTaskRequestDto
            {
                Title = "EditValidTask",
                Status = Status.Completed,
                Priority = Priority.Medium,
                UserId = Helper.ExistingUser.Id
            };
            var existingUserId = Helper.ExistingUser.Id;
            var existingTaskId = Helper.ExistingTask.Id;

            // Act
            await _userTaskService.EditTaskAsync(existingTaskId, taskToUpdate, existingUserId);

            // Assert
            var editedTask = await _userTaskService.GetTaskByIdAsync(existingTaskId, existingUserId);
            Assert.Equal(Priority.Medium, editedTask.Priority);
        }

        [Fact]
        public async Task EditTaskAsync_ValidTask_ByNonExistentUser_ReturnsNotUpdatedTask()
        {
            // Arrange
            var taskToUpdate = new UpdateUserTaskRequestDto
            {
                Title = "EditValidTask",
                Status = Status.Completed,
                Priority = Priority.Medium,
                UserId = Helper.ExistingUser.Id
            };
            var nonExistentUserId = Guid.Empty;
            var existingUserId = Helper.ExistingUser.Id;
            var existingTaskId = Helper.ExistingTask.Id;

            // Act
            await _userTaskService.EditTaskAsync(existingTaskId, taskToUpdate, nonExistentUserId);

            // Assert
            var editedTask = await _userTaskService.GetTaskByIdAsync(existingTaskId, existingUserId);
            Assert.Equal(Priority.Low, editedTask.Priority);
        }
        #endregion

        #region DeleteTaskAsync
        [Theory]
        [MemberData(nameof(Helper.UserIds), MemberType = typeof(Helper))]
        public async Task DeleteTaskAsync_TaskThatDoesNotExists_ReturnsNull(Guid userId)
        {
            // Arrange
            var nonExistentTaskId = Guid.Empty;

            // Act
            var deletedTask = await _userTaskService.DeleteTaskAsync(nonExistentTaskId, userId);

            // Assert
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task DeleteTaskAsync_TaskThatExists_ByExistingUser_ReturnsNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            await _userTaskService.DeleteTaskAsync(existingTaskId, existingUserId);

            // Assert
            var deletedTask = await _userTaskService.GetTaskByIdAsync(existingTaskId, existingUserId);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task DeleteTaskAsync_TaskThatExists_ByNonExistentUser_ReturnsNotNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var existingUserId = Helper.ExistingUser.Id;
            var nonExistentUserId = Guid.Empty;

            // Act
            await _userTaskService.DeleteTaskAsync(existingTaskId, nonExistentUserId);

            // Assert
            var deletedTask = await _userTaskService.GetTaskByIdAsync(existingTaskId, existingUserId);
            Assert.NotNull(deletedTask);
        }
        #endregion
    }
}
