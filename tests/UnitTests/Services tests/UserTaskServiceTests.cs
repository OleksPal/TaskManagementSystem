using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        #region GetTaskById
        [Fact]
        public async Task GetTaskById_TaskDoesNotExists_ReturnsNull()
        {
            var task = await _userTaskService.GetTaskById(Guid.Empty);

            Assert.Null(task);
        }

        [Fact]
        public async Task GetTaskById_TaskThatExists_ReturnsNotNull()
        {
            var addedTask = await _userTaskService.AddTask(validTaskObject);

            var task = await _userTaskService.GetTaskById(addedTask.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region AddTask
        [Fact]
        public async Task AddTask_Null_ReturnsArgumentNullException()
        {
            UserTask taskToAdd = null;

            Func<Task> act = () => _userTaskService.AddTask(taskToAdd);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task AddTask_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask taskToAdd = new UserTask();

            Func<Task> act = () => _userTaskService.AddTask(taskToAdd);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task AddTask_ValidTask_ReturnsNotNull()
        {
            UserTask taskToAdd = validTaskObject;

            await _userTaskService.AddTask(taskToAdd);
            var addedTask = await _userTaskService.GetTaskById(taskToAdd.Id);

            Assert.NotNull(addedTask);
        }
        #endregion

        #region EditTask
        [Fact]
        public async Task EditTask_Null_ReturnsNullReferenceException()
        {
            UserTask taskToEdit = null;

            Func<Task> act = () => _userTaskService.EditTask(taskToEdit);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task EditTask_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask taskToEdit = new UserTask();

            Func<Task> act = () => _userTaskService.AddTask(taskToEdit);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task EditTask_ValidTask_ReturnsPriorityMedium()
        {
            var taskToEdit = await _userTaskService.AddTask(validTaskObject);
            taskToEdit.Priority = Priority.Medium;

            var editedTask = await _userTaskService.EditTask(taskToEdit);

            Assert.Equal(Priority.Medium, editedTask.Priority);
        }
        #endregion

        #region DeleteTask
        [Fact]
        public async Task DeleteTask_TaskThatDoesNotExists_ReturnsNull()
        {
            var deletedTask = await _userTaskService.DeleteTask(Guid.Empty);

            Assert.Null(deletedTask);
        }

        [Fact]
        public async void DeleteTask_TaskThatExists_ReturnsNull()
        {
            var taskToAdd = validTaskObject;
            var addedTask = await _userTaskService.AddTask(taskToAdd);

            await _userTaskService.DeleteTask(addedTask.Id);
            var deletedTask = await _userTaskService.GetTaskById(addedTask.Id);

            Assert.Null(deletedTask);
        }
        #endregion
    }
}
