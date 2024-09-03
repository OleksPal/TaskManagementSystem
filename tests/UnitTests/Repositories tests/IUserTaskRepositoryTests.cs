using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using TaskManagementSystem.Data;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.UnitTests
{
    public class IUserTaskRepositoryTests
    {
        protected readonly DbConnection _connection;
        protected readonly DbContextOptions<TaskManagementContext> _contextOptions;

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
        public async void GetAll()
        {
            using var context = CreateContext();
            var repository = new UserTaskRepository(context);

            var list = await repository.GetAll();

            Assert.NotEmpty(list);
        }
        #endregion
    }
}