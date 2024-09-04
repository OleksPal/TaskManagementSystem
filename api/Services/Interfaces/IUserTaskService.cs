using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services.Interfaces
{
    public interface IUserTaskService
    {
        Task<ICollection<UserTask>> GetAllTasks();
        Task<UserTask> GetTaskById(Guid id);
        Task<UserTask> AddTask(UserTask task);
        Task<UserTask> EditTask(UserTask task);
        Task<UserTask> DeleteTask(Guid id);
    }
}
