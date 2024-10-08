﻿using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories.Interfaces
{
    public interface IUserTaskRepository
    {
        Task<ICollection<UserTask>> GetAllAsync(Guid userId, QueryObject query);
        Task<UserTask?> GetByIdAsync(Guid id, Guid userId);
        Task<UserTask> InsertAsync(UserTask task);
        Task<UserTask> UpdateAsync(UserTask task);
        Task<UserTask?> DeleteAsync(Guid id, Guid userId);
    }
}
