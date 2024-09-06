using Microsoft.EntityFrameworkCore;
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

        #region GetAllTasks
        [Fact]
        public async Task GetAllTasks_ReturnsNotEmptyCollection()
        {
            var taskList = await _userTaskService.GetAllTasksAsync();

            Assert.NotEmpty(taskList);
        }
        #endregion

        #region GetTaskById
        [Fact]
        public async Task GetTaskById_TaskDoesNotExists_ReturnsNull()
        {
            var task = await _userTaskService.GetTaskByIdAsync(Guid.Empty);

            Assert.Null(task);
        }

        [Fact]
        public async Task GetTaskById_TaskThatExists_ReturnsNotNull()
        {
            var addedTask = await _userTaskService.AddTaskAsync(validTaskObject);

            var task = await _userTaskService.GetTaskByIdAsync(addedTask.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region AddTask
        [Fact]
        public async Task AddTask_Null_ReturnsArgumentNullException()
        {
            UserTask taskToAdd = null;

            Func<Task> act = () => _userTaskService.AddTaskAsync(taskToAdd);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task AddTask_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask taskToAdd = new UserTask();

            Func<Task> act = () => _userTaskService.AddTaskAsync(taskToAdd);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task AddTask_ValidTask_ReturnsNotNull()
        {
            UserTask taskToAdd = validTaskObject;

            await _userTaskService.AddTaskAsync(taskToAdd);
            var addedTask = await _userTaskService.GetTaskByIdAsync(taskToAdd.Id);

            Assert.NotNull(addedTask);
        }
        #endregion

        #region EditTask
        [Fact]
        public async Task EditTask_Null_ReturnsNullReferenceException()
        {
            UserTask taskToEdit = null;

            Func<Task> act = () => _userTaskService.EditTaskAsync(taskToEdit);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task EditTask_TaskThatDoesNotExists_ReturnsDbUpdateConcurrencyException()
        {
            UserTask task = validTaskObject;

            Func<Task> act = () => _userTaskService.EditTaskAsync(task);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(act);
        }

        [Fact]
        public async Task EditTask_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            UserTask taskToEdit = new UserTask();

            Func<Task> act = () => _userTaskService.AddTaskAsync(taskToEdit);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task EditTask_ValidTask_ReturnsUpdatedTask()
        {
            var taskToEdit = await _userTaskService.AddTaskAsync(validTaskObject);
            taskToEdit.Priority = Priority.Medium;

            await _userTaskService.EditTaskAsync(taskToEdit);
            var editedTask = await _userTaskService.GetTaskByIdAsync(taskToEdit.Id);

            Assert.Equal(Priority.Medium, editedTask.Priority);
        }
        #endregion

        #region DeleteTask
        [Fact]
        public async Task DeleteTask_TaskThatDoesNotExists_ReturnsNull()
        {
            var deletedTask = await _userTaskService.DeleteTaskAsync(Guid.Empty);

            Assert.Null(deletedTask);
        }

        [Fact]
        public async void DeleteTask_TaskThatExists_ReturnsNull()
        {
            var taskToAdd = validTaskObject;
            var addedTask = await _userTaskService.AddTaskAsync(taskToAdd);

            await _userTaskService.DeleteTaskAsync(addedTask.Id);
            var deletedTask = await _userTaskService.GetTaskByIdAsync(addedTask.Id);

            Assert.Null(deletedTask);
        }
        #endregion
    }
}
