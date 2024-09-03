using TaskManagementSystem.Models;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.UnitTests
{
    public class UserTaskServiceTests
    {
        protected readonly IUserTaskService _userTaskService;

        protected readonly UserTask validTaskObject = new UserTask
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Status = Status.Completed,
            Priority = Priority.Low,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public UserTaskServiceTests()
        {
            _userTaskService = Helper.GetRequiredService<IUserTaskService>()
                ?? throw new ArgumentNullException(nameof(IUserTaskService));
        }
    }
}
