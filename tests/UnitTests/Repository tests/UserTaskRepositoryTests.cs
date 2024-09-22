using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    [Collection("TestCollection")]
    public class UserTaskRepositoryTests
    {
        public static IEnumerable<object[]> UserIds => new List<object[]>
        {
            new object[] { Guid.Empty },
            new object[] { Helper.ExistingUser.Id }
        };

        protected readonly IUserTaskRepository _userTaskRepository;

        public UserTaskRepositoryTests()
        {
            _userTaskRepository = Helper.GetRequiredService<IUserTaskRepository>()
                ?? throw new ArgumentNullException(nameof(IUserTaskRepository));
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ByExistingUser_ReturnsNotEmpty()
        {
            // Arrange
            var query = new QueryObject();
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            var taskList = await _userTaskRepository.GetAllAsync(existingUserId, query);

            // Assert
            Assert.NotEmpty(taskList);
        }

        [Fact]
        public async Task GetAllAsync_ByNonExistentUser_ReturnsEmpty()
        {
            // Arrange
            var query = new QueryObject();
            var nonExistentUserId = Guid.Empty;

            // Act
            var taskList = await _userTaskRepository.GetAllAsync(nonExistentUserId, query);

            // Assert
            Assert.Empty(taskList);
        }
        #endregion

        #region GetByIdAsync
        [Theory]
        [MemberData(nameof(UserIds))]
        public async Task GetByIdAsync_TaskDoesNotExists_ReturnsNull(Guid userId)
        {
            // Arrange
            var nonExistentTaskId = Guid.Empty;

            // Act
            var task = await _userTaskRepository.GetByIdAsync(nonExistentTaskId, userId);

            // Assert
            Assert.Null(task);
        }

        [Fact]
        public async Task GetByIdAsync_TaskExists_ByExistingUser_ReturnsNotNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            var task = await _userTaskRepository.GetByIdAsync(existingTaskId, existingUserId);

            // Assert
            Assert.NotNull(task);
        }

        [Fact]
        public async Task GetByIdAsync_TaskExists_ByNonExistentUser_ReturnsNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var nonExistentUserId = Guid.Empty;

            // Act
            var task = await _userTaskRepository.GetByIdAsync(existingTaskId, nonExistentUserId);

            // Assert
            Assert.Null(task);
        }
        #endregion

        #region InsertAsync
        [Fact]
        public async Task InsertAsync_Null_ReturnsArgumentNullException()
        {
            // Arrange
            UserTask taskToInsert = null;

            // Act
            Func<Task> act = () => _userTaskRepository.InsertAsync(taskToInsert);

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task InsertAsync_TaskWithIdThatAlreadyExists_ReturnsArgumentException()
        {
            // Arrange
            UserTask taskToInsert = Helper.ExistingTask;

            // Act
            Func<Task> act = () => _userTaskRepository.InsertAsync(taskToInsert);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task InsertAsync_InvalidTaskWithoutRequiredProperties_ReturnsDbUpdateException()
        {
            // Arrange
            var taskToInsert = new UserTask();

            // Act
            Func<Task> act = () => _userTaskRepository.InsertAsync(taskToInsert);

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task InsertAsync_ValidTask_ReturnsNotNull()
        {
            // Arrange
            var taskToInsert = new UserTask
            {
                Id = Guid.NewGuid(),
                Title = "ValidInsertTest",
                Status = Status.InProgress,
                Priority = Priority.Low,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = Helper.ExistingUser.Id
            };
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            await _userTaskRepository.InsertAsync(taskToInsert);

            // Assert
            var insertedTask = await _userTaskRepository.GetByIdAsync(taskToInsert.Id, existingUserId);
            Assert.NotNull(insertedTask);
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_Null_ReturnsNullReferenceException()
        {
            // Arrange
            UserTask taskToUpdate = null;

            // Act
            Func<Task> act = () => _userTaskRepository.UpdateAsync(taskToUpdate);

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task UpdateAsync_TaskThatDoesNotExists_ReturnsDbUpdateConcurrencyException()
        {
            // Arrange
            UserTask taskToUpdate = new UserTask
            {
                Id = Guid.NewGuid(),
                Title = "UpdateNonExistingTaskTest",
                Status = Status.InProgress,
                Priority = Priority.Low,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = Helper.ExistingUser.Id
            };

            // Act
            Func<Task> act = () => _userTaskRepository.UpdateAsync(taskToUpdate);

            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(act);
        }

        [Fact]
        public async Task UpdateAsync_ValidTask_ReturnsUpdatedTask()
        {
            // Arrange
            var existingUserId = Helper.ExistingUser.Id;
            var existingTask = Helper.ExistingTask;
            existingTask.Priority = Priority.Medium;

            // Act
            await _userTaskRepository.UpdateAsync(Helper.ExistingTask);

            // Assert
            var updatedTask = await _userTaskRepository.GetByIdAsync(existingTask.Id, existingUserId);
            Assert.Equal(Priority.Medium, updatedTask.Priority);
        }
        #endregion

        #region DeleteAsync
        [Theory]
        [MemberData(nameof(UserIds))]
        public async Task DeleteAsync_TaskThatDoesNotExists_ReturnsNull(Guid userId)
        {
            // Arrange
            var nonExistentTaskId = Guid.Empty;

            // Act
            var deletedTask = await _userTaskRepository.DeleteAsync(nonExistentTaskId, userId);

            // Assert
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task DeleteAsync_TaskThatExists_ByExistingUser_ReturnsNull()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var existingUserId = Helper.ExistingUser.Id;

            // Act
            await _userTaskRepository.DeleteAsync(existingTaskId, existingUserId);

            // Assert
            var deletedTask = await _userTaskRepository.GetByIdAsync(existingTaskId, existingUserId);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task DeleteAsync_TaskThatExists_ByNonExistentUser_ReturnsNotDeletedTask()
        {
            // Arrange
            var existingTaskId = Helper.ExistingTask.Id;
            var existingUserId = Helper.ExistingUser.Id;
            var nonExistentUserId = Guid.Empty;

            // Act
            await _userTaskRepository.DeleteAsync(existingTaskId, nonExistentUserId);

            // Assert
            var deletedTask = await _userTaskRepository.GetByIdAsync(existingTaskId, existingUserId);
            Assert.NotNull(deletedTask);
        }
        #endregion
    }
}