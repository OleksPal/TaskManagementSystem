using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories.Interfaces
{
    public interface IUserTaskRepository
    {
        Task<ICollection<UserTask>> GetAll();
        Task<UserTask> GetById(Guid id);
        Task<UserTask> Insert(UserTask task);
        Task<UserTask> Update(UserTask task);
        Task<UserTask> Delete(Guid id);
    }
}
