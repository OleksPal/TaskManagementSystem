using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Repositories.Interfaces;
using TaskManagementSystem.Services;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    public static class Helper
    {
        public static User ExistingUser { get; private set; }
        public static UserTask ExistingTask { get; private set; }

        public static IEnumerable<object[]> UserIds => new List<object[]>
        {
            new object[] { Guid.Empty },
            new object[] { Helper.ExistingUser.Id }
        };

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
            SetExistingEntities(context);

            return requiredService;
        }

        private static void AddData(TaskManagementContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        private static void SetExistingEntities(TaskManagementContext context)
        {
            ExistingUser = context.Users.FirstOrDefault();
            ExistingTask = context.Tasks.FirstOrDefault();

            if (ExistingUser is null)
                throw new NullReferenceException("Users table is empty");
            else if (ExistingTask is null)
                throw new NullReferenceException("Tasks table is empty");
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }        
}
