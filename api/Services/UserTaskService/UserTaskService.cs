using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Mappers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories.Interfaces;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly IUserTaskRepository _taskRepository;

        public UserTaskService(IUserTaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<ICollection<UserTaskDto>?> GetAllTasksAsync(Guid userId, QueryObject query)
        {
            var taskList = await _taskRepository.GetAllAsync(userId, query);

            if (taskList is null)
                return null;

            return taskList.Select(task => task.ToUserTaskDTO()).ToList();
        }

        public async Task<UserTaskDto?> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, userId);

            if (task is null)
                return null;

            return task.ToUserTaskDTO();
        }

        public async Task<UserTaskDto> AddTaskAsync(CreateUserTaskRequestDto createTaskDto)
        {
            var task = createTaskDto.ToUserTask();
            await _taskRepository.InsertAsync(task);

            return task.ToUserTaskDTO();
        }

        public async Task<UserTaskDto?> EditTaskAsync(Guid id, UpdateUserTaskRequestDto updateTaskDto, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task is null) 
                return null;

            task = updateTaskDto.ToUserTask(id, task.CreatedAt);
            await _taskRepository.UpdateAsync(task);

            return task.ToUserTaskDTO();
        }

        public async Task<UserTask?> DeleteTaskAsync(Guid id, Guid userId)
        {
            return await _taskRepository.DeleteAsync(id, userId);
        }
    }
}
