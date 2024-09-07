using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories.Interfaces
{
    public interface IUserTaskRepository
    {
        Task<ICollection<UserTask>> GetAllAsync(QueryObject query);
        Task<UserTask?> GetByIdAsync(Guid id);
        Task<UserTask> InsertAsync(UserTask task);
        Task<UserTask> UpdateAsync(UserTask task);
        Task<UserTask?> DeleteAsync(Guid id);
    }
}
