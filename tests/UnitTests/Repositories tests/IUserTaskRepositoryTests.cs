using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.UnitTests
{
    public class IUserTaskRepositoryTests
    {
        protected readonly DbConnection _connection;
        protected readonly DbContextOptions<TaskManagementContext> _contextOptions;

        protected readonly UserTask validTaskObject = new UserTask
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Status = Status.Completed,
            Priority = Priority.Low,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public IUserTaskRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<TaskManagementContext>()
            .UseSqlite(_connection)
                .Options;

            using var context = new TaskManagementContext(_contextOptions);

            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        public TaskManagementContext CreateContext() => new TaskManagementContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        #region GetAll
        [Fact]
        public async Task GetAll_ReturnsNotEmpty()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);

            var list = await repository.GetAll();

            Assert.NotEmpty(list);
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetById_ItemDoesNotExists_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);

            var task = await repository.GetById(Guid.Empty);

            Assert.Null(task);
        }

        [Fact]
        public async Task GetById_ItemExists_ReturnsNotNull()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            var addedTask = await repository.Insert(validTaskObject);

            var task = await repository.GetById(addedTask.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region Insert
        [Fact]
        public async Task Insert_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            UserTask task = new UserTask();

            Func<Task> act = () => repository.Insert(task);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Insert_Null_ReturnsArgumentNullException()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            UserTask task = null;

            Func<Task> act = () => repository.Insert(task);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Insert_ValidTask_ReturnsSameTask()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            UserTask task = validTaskObject;

            var addedTask = await repository.Insert(task);

            Assert.Equal(task.Id, addedTask.Id);
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_Null_ReturnsNullReferenceException()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            UserTask task = null;

            Func<Task> act = () => repository.Update(task);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task Update_InvalidObject_ReturnsDbUpdateException()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            UserTask task = new UserTask();

            Func<Task> act = () => repository.Update(task);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Update_ValidTask_ReturnsSameTask()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            UserTask task = await repository.Insert(validTaskObject);
            task.Priority = Priority.Medium;

            var updatedTask = await repository.Update(task);

            Assert.Equal(Priority.Medium, updatedTask.Priority);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_RecordThatDoesNotExists_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);

            var deletedItem = await repository.Delete(Guid.Empty);

            Assert.Null(deletedItem);
        }

        [Fact]
        public async Task Delete_RecordThatExists_ReturnsNotNull()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);
            var task = await repository.Insert(validTaskObject);

            var deletedItem = await repository.Delete(task.Id);

            Assert.NotNull(deletedItem);
        }
        #endregion
    }
}