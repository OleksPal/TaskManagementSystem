using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services.Interfaces
{
    public interface IUserTaskService
    {
        Task<UserTask> GetTaskById(Guid id);
        Task<UserTask> AddTask(Task task);
        Task<UserTask> EditTask(Task task);
        Task<UserTask> DeleteTask(Guid id);
    }
}
