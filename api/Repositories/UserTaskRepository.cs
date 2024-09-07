﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<ICollection<UserTask>> GetAllAsync(QueryObject query)
        {
            var tasks = _context.Tasks.AsQueryable();

            if (query.Status is not null)
                tasks = tasks.Where(task => task.Status == query.Status);

            if (query.DueDate is not null)
                tasks = tasks.Where(task => task.DueDate == query.DueDate);

            if (query.Priority is not null)
                tasks = tasks.Where(task => task.Priority == query.Priority);

            return await tasks.ToListAsync();
        }

        public async Task<UserTask?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<UserTask> InsertAsync(UserTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<UserTask> UpdateAsync(UserTask task)
        {
            _context.ChangeTracker.Clear();

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
