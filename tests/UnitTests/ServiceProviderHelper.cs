using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Data;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Repositories.Interfaces;
using TaskManagementSystem.Services;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    public static class Helper
    {
        public static Guid existingUserId;
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            #region Repositories
            services.AddScoped<IUserTaskRepository, UserTaskRepository>();
            #endregion

            #region Services
            services.AddScoped<IUserTaskService, UserTaskService>();
            #endregion

            services.AddDbContext<TaskManagementContext>(o => o.UseInMemoryDatabase("TestDB"));

            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            var requiredService = provider.GetRequiredService<T>();

            var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();

            AddData(context);

            existingUserId = GetExistingUserId(context);

            return requiredService;
        }

        private static void AddData(TaskManagementContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        private static Guid GetExistingUserId(TaskManagementContext context)
        {
            var users = context.Users.ToList();
            return users.First().Id;
        }
    }        
}
