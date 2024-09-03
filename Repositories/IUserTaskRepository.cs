using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public interface IUserTaskRepository
    {
        Task<ICollection<UserTask>> GetAll();
        Task<UserTask> GetById(int id);
        Task<UserTask> Insert(UserTask task);
        Task<UserTask> Update(UserTask task);
        Task<UserTask> Delete(int id);
    }
}
