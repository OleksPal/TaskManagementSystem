using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Helpers;
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

        public async Task<ICollection<UserTask>> GetAllAsync(Guid userId, QueryObject query)
        {
            var tasks = _context.Tasks.AsQueryable().Where(task => task.UserId == userId || task.UserId == null);

            // Filtering
            if (query.Status is not null)
                tasks = tasks.Where(task => task.Status == query.Status);

            if (query.DueDate is not null)
                tasks = tasks.Where(task => task.DueDate == query.DueDate);

            if (query.Priority is not null)
                tasks = tasks.Where(task => task.Priority == query.Priority);

            // Sorting
            if (!String.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("DueDate", StringComparison.OrdinalIgnoreCase))
                    tasks = query.IsDescending ? 
                        tasks.OrderByDescending(task => task.DueDate) : 
                        tasks.OrderBy(task => task.DueDate);
                else if (query.SortBy.Equals("Priority", StringComparison.OrdinalIgnoreCase))
                    tasks = query.IsDescending ?
                        tasks.OrderByDescending(task => task.Priority) :
                        tasks.OrderBy(task => task.Priority);
            }

            // Pagination
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await tasks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<UserTask?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<UserTask> InsertAsync(UserTask task)
        {
            task.CreatedAt = DateTime.Now;
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<UserTask> UpdateAsync(UserTask task)
        {
            _context.ChangeTracker.Clear();

            task.UpdatedAt = DateTime.Now;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            
            return task;
        }

        public async Task<UserTask?> DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task is null)
                return null;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();                

            return task;
        }
    }
}
