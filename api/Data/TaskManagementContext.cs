using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data
{
    public class TaskManagementContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole<Guid>> roles = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid> {
                    Name = "User",
                    NormalizedName = "USER"
                }
            };

            builder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}
