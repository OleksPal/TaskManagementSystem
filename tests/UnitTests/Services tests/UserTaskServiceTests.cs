using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Mappers;
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

        public readonly CreateUserTaskRequestDto _userTaskCreateDto = new()
        {
            Title = "CreateTest",
            Status = Status.Completed,
            Priority = Priority.Low,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public readonly UpdateUserTaskRequestDto _userTaskUpdateDto = new()
        {
            Title = "UpdateTest",
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
            var query = new QueryObject();
            var taskList = await _userTaskService.GetAllTasksAsync(query);

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
            var addedTask = await _userTaskService.AddTaskAsync(_userTaskCreateDto);

            var task = await _userTaskService.GetTaskByIdAsync(addedTask.Id);

            Assert.NotNull(task);
        }
        #endregion

        #region AddTask
        [Fact]
        public async Task AddTask_Null_ReturnsNullReferenceException()
        {
            CreateUserTaskRequestDto taskToAdd = null;

            Func<Task> act = () => _userTaskService.AddTaskAsync(taskToAdd);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task AddTask_ValidTaskWithDefaultValues_ReturnsDbUpdateException()
        {
            CreateUserTaskRequestDto taskToAdd = new();

            Func<Task> act = () => _userTaskService.AddTaskAsync(taskToAdd);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task AddTask_ValidTask_ReturnsNotNull()
        {
            CreateUserTaskRequestDto taskToAdd = _userTaskCreateDto;

            var task = await _userTaskService.AddTaskAsync(_userTaskCreateDto);
            var addedTask = await _userTaskService.GetTaskByIdAsync(task.Id);

            Assert.NotNull(addedTask);
        }
        #endregion

        #region EditTask
        [Fact]
        public async Task EditTask_Null_ReturnsNullReferenceException()
        {
            var task = await _userTaskService.AddTaskAsync(_userTaskCreateDto);
            UpdateUserTaskRequestDto taskToEdit = null;

            Func<Task> act = () => _userTaskService.EditTaskAsync(task.Id, taskToEdit);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async Task EditTask_TaskThatDoesNotExists_ReturnsNull()
        {
            UpdateUserTaskRequestDto task = _userTaskUpdateDto;

            var taskDto = await _userTaskService.EditTaskAsync(Guid.Empty, task);

            Assert.Null(taskDto);
        }

        [Fact]
        public async Task EditTask_ValidTask_ReturnsUpdatedTask()
        {
            var taskToEdit = await _userTaskService.AddTaskAsync(_userTaskCreateDto);
            _userTaskUpdateDto.Priority = Priority.Medium;

            await _userTaskService.EditTaskAsync(taskToEdit.Id, _userTaskUpdateDto);
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
            var addedTask = await _userTaskService.AddTaskAsync(_userTaskCreateDto);

            await _userTaskService.DeleteTaskAsync(addedTask.Id);
            var deletedTask = await _userTaskService.GetTaskByIdAsync(addedTask.Id);

            Assert.Null(deletedTask);
        }
        #endregion
    }
}
