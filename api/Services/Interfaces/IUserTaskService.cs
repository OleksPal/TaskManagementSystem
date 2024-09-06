using TaskManagementSystem.DTOs;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services.Interfaces
{
    public interface IUserTaskService
    {
        Task<ICollection<UserTaskDto>?> GetAllTasksAsync();
        Task<UserTaskDto?> GetTaskByIdAsync(Guid id);
        Task<UserTaskDto> AddTaskAsync(CreateUserTaskRequestDto createTaskDto);
        Task<UserTaskDto?> EditTaskAsync(Guid id, UpdateUserTaskRequestDto updateTaskDto);
        Task<UserTask?> DeleteTaskAsync(Guid id);
    }
}
