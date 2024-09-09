using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services.Interfaces
{
    public interface IUserTaskService
    {
        Task<ICollection<UserTaskDto>?> GetAllTasksAsync(Guid userId, QueryObject query);
        Task<UserTaskDto?> GetTaskByIdAsync(Guid taskId, Guid userId);
        Task<UserTaskDto> AddTaskAsync(CreateUserTaskRequestDto createTaskDto);
        Task<UserTaskDto?> EditTaskAsync(Guid id, UpdateUserTaskRequestDto updateTaskDto, Guid userId);
        Task<UserTask?> DeleteTaskAsync(Guid id, Guid userId);
    }
}
