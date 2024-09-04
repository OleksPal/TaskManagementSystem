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

        public async Task<ICollection<UserTask>> GetAllTasks()
        {
            return await _taskRepository.GetAll();
        }

        public async Task<UserTask> GetTaskById(Guid id)
        {
            return await _taskRepository.GetById(id);
        }

        public async Task<UserTask> AddTask(UserTask task)
        {
            return await _taskRepository.Insert(task);
        }

        public async Task<UserTask> EditTask(UserTask task)
        {
            return await _taskRepository.Update(task);
        }

        public async Task<UserTask> DeleteTask(Guid id)
        {
            return await _taskRepository.Delete(id);
        }
    }
}
