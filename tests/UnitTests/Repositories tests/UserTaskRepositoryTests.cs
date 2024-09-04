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
            var list = await _userTaskRepository.GetAll();

            Assert.NotEmpty(list);
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetById_ItemDoesNotExists_ReturnsNull()
        {
            var task = await _userTaskRepository.GetById(Guid.Empty);

            Assert.Null(task);
        }

        [Fact]
        public async Task GetById_ItemExists_ReturnsNotNull()
        {
            var addedTask = await _userTaskRepository.Insert(validTaskObject);

            var task = await _userTaskRepository.GetById(addedTask.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region Insert
        [Fact]
        public async Task Insert_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask task = new UserTask();

            Func<Task> act = () => _userTaskRepository.Insert(task);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Insert_Null_ReturnsArgumentNullException()
        {
            UserTask task = null;

            Func<Task> act = () => _userTaskRepository.Insert(task);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Insert_ValidTask_ReturnsNotNull()
        {
            await _userTaskRepository.Insert(validTaskObject);

            var insertedTask = await _userTaskRepository.GetById(validTaskObject.Id);

            Assert.NotNull(insertedTask);
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_Null_ReturnsNullReferenceException()
        {
            UserTask task = null;

            Func<Task> act = () => _userTaskRepository.Update(task);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task Update_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask task = new UserTask();

            Func<Task> act = () => _userTaskRepository.Update(task);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Update_ValidTask_ReturnsSameTask()
        {
            UserTask task = await _userTaskRepository.Insert(validTaskObject);
            task.Priority = Priority.Medium;

            var updatedTask = await _userTaskRepository.Update(task);

            Assert.Equal(Priority.Medium, updatedTask.Priority);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_RecordThatDoesNotExists_ReturnsNull()
        {
            var deletedItem = await _userTaskRepository.Delete(Guid.Empty);

            Assert.Null(deletedItem);
        }

        [Fact]
        public async Task Delete_RecordThatExists_ReturnsNotNull()
        {
            var task = await _userTaskRepository.Insert(validTaskObject);

            var deletedItem = await _userTaskRepository.Delete(task.Id);

            Assert.NotNull(deletedItem);
        }
        #endregion
    }
}