﻿using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TaskManagementContext context)
        {
            if (context.Users.Any() && context.Tasks.Any())
                return;

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                Email = "test@gmail.com",                
                PasswordHash = String.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var userTask = new UserTask
            {
                Id = Guid.NewGuid(),
                Title = "TestTask",
                Status = Status.Completed,
                Priority = Priority.Low,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = user.Id
            };

            context.Users.Add(user);
            context.Tasks.Add(userTask);

            context.SaveChanges();
        }
    }
}
