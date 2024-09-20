using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    public class UserTaskRepositoryTests
    {
        protected readonly IUserTaskRepository _userTaskRepository;

        protected readonly UserTask validTaskObject = new()
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
            var query = new QueryObject();
            var taskList = await _userTaskRepository.GetAllAsync(Helper.ExistingUser.Id, query);

            Assert.NotEmpty(taskList);
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetById_TaskDoesNotExists_ReturnsNull()
        {
            var task = await _userTaskRepository.GetByIdAsync(Guid.Empty, Helper.ExistingUser.Id);

            Assert.Null(task);
        }

        [Fact]
        public async Task GetById_TaskExists_ReturnsNotNull()
        {
            var task = await _userTaskRepository.GetByIdAsync(Helper.ExistingTask.Id, Helper.ExistingUser.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region Insert
        [Fact]
        public async Task Insert_Null_ReturnsArgumentNullException()
        {
            UserTask taskToInsert = null;

            Func<Task> act = () => _userTaskRepository.InsertAsync(taskToInsert);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task Insert_ValidTask_ReturnsNotNull()
        {
            var taskToInsert = validTaskObject;

            await _userTaskRepository.InsertAsync(taskToInsert);
            var insertedTask = await _userTaskRepository.GetByIdAsync(taskToInsert.Id, Helper.ExistingUser.Id);

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
            Helper.ExistingTask.Priority = Priority.Medium;

            await _userTaskRepository.UpdateAsync(Helper.ExistingTask);
            var updatedTask = await _userTaskRepository.GetByIdAsync(Helper.ExistingTask.Id, Helper.ExistingUser.Id);

            Assert.Equal(Priority.Medium, updatedTask.Priority);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_TaskThatDoesNotExists_ReturnsNull()
        {
            var deletedTask = await _userTaskRepository.DeleteAsync(Guid.Empty, Helper.ExistingUser.Id);

            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task Delete_TaskThatExists_ReturnsNull()
        {
            await _userTaskRepository.DeleteAsync(Helper.ExistingTask.Id, Helper.ExistingUser.Id);
            var deletedTask = await _userTaskRepository.GetByIdAsync(Helper.ExistingTask.Id, Helper.ExistingUser.Id);

            Assert.Null(deletedTask);
        }
        #endregion
    }
}