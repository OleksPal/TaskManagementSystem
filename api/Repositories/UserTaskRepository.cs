using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories.Interfaces;

namespace TaskManagementSystem.Repositories
{
    public class UserTaskRepository : IUserTaskRepository
    {
        protected readonly TaskManagementContext _context;

        public UserTaskRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<ICollection<UserTask>> GetAll()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<UserTask> GetById(Guid id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        }

        public async Task<UserTask> Insert(UserTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<UserTask> Update(UserTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            
            return task;
        }

        public async Task<UserTask> Delete(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(task => task.Id == id);

            if (task is not null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }                

            return task;
        }
    }
}
