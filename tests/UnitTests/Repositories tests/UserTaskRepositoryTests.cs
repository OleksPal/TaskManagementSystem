using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    public class UserTaskRepositoryTests
    {
        protected readonly IUserTaskRepository _userTaskRepository;

        protected readonly UserTask validTaskObject = new UserTask
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Status = Status.Completed,
            Priority = Priority.Low,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public UserTaskRepositoryTests()
        {
            _userTaskRepository = Helper.GetRequiredService<IUserTaskRepository>()
                ?? throw new ArgumentNullException(nameof(IUserTaskRepository));
        }

        #region GetAll
        [Fact]
        public async Task GetAll_ReturnsNotEmpty()
        {
            var taskList = await _userTaskRepository.GetAllAsync();

            Assert.NotEmpty(taskList);
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetById_TaskDoesNotExists_ReturnsNull()
        {
            var task = await _userTaskRepository.GetByIdAsync(Guid.Empty);

            Assert.Null(task);
        }

        [Fact]
        public async Task GetById_TaskExists_ReturnsNotNull()
        {
            var insertedTask = await _userTaskRepository.InsertAsync(validTaskObject);

            var task = await _userTaskRepository.GetByIdAsync(insertedTask.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region Insert
        [Fact]
        public async Task Insert_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask taskToInsert = new UserTask();

            Func<Task> act = () => _userTaskRepository.InsertAsync(taskToInsert);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Insert_Null_ReturnsArgumentNullException()
        {
            UserTask taskToInsert = null;

            Func<Task> act = () => _userTaskRepository.InsertAsync(taskToInsert);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Insert_ValidTask_ReturnsNotNull()
        {
            var taskToInsert = await _userTaskRepository.InsertAsync(validTaskObject);
            var insertedTask = await _userTaskRepository.GetByIdAsync(taskToInsert.Id);

            Assert.NotNull(insertedTask);
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_Null_ReturnsNullReferenceException()
        {
            UserTask task = null;

            Func<Task> act = () => _userTaskRepository.UpdateAsync(task);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task Update_TaskThatDoesNotExists_ReturnsDbUpdateConcurrencyException()
        {
            UserTask task = validTaskObject;

            Func<Task> act = () => _userTaskRepository.UpdateAsync(task);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(act);
        }

        [Fact]
        public async Task Update_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask task = new UserTask();

            Func<Task> act = () => _userTaskRepository.UpdateAsync(task);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Update_ValidTask_ReturnsUpdatedTask()
        {
            var insertedTask = await _userTaskRepository.InsertAsync(validTaskObject);
            insertedTask.Priority = Priority.Medium;

            await _userTaskRepository.UpdateAsync(insertedTask);
            var updatedTask = await _userTaskRepository.GetByIdAsync(insertedTask.Id);

            Assert.Equal(Priority.Medium, updatedTask.Priority);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_TaskThatDoesNotExists_ReturnsNull()
        {
            var deletedTask = await _userTaskRepository.DeleteAsync(Guid.Empty);

            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task Delete_TaskThatExists_ReturnsNull()
        {
            var taskToDelete = await _userTaskRepository.InsertAsync(validTaskObject);

            await _userTaskRepository.DeleteAsync(taskToDelete.Id);
            var deletedTask = await _userTaskRepository.GetByIdAsync(taskToDelete.Id);

            Assert.Null(deletedTask);
        }
        #endregion
    }
}