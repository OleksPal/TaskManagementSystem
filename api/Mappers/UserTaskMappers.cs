using TaskManagementSystem.Models;
using TaskManagementSystem.Models.DTOs;

namespace TaskManagementSystem.Mappers
{
    public static class UserTaskMappers
    {
        public static UserTaskDTO ToUserTaskDTO(this UserTask userTask)
        {
            return new UserTaskDTO
            {
                Title = userTask.Title,
                Description = userTask.Description,
                DueDate = userTask.DueDate,
                Status = userTask.Status,
                Priority = userTask.Priority,
                CreatedAt = userTask.CreatedAt,
                UpdatedAt = userTask.UpdatedAt,
                UserId = userTask.UserId,
                User = userTask.User
            };
        }

        public static UserTask ToUserTask(this UserTaskDTO userTaskDTO)
        {
            return new UserTask
            {
                Id = Guid.NewGuid(),
                Title = userTaskDTO.Title,
                Description = userTaskDTO.Description,
                DueDate = userTaskDTO.DueDate,
                Status = userTaskDTO.Status,
                Priority = userTaskDTO.Priority,
                CreatedAt = userTaskDTO.CreatedAt,
                UpdatedAt = userTaskDTO.UpdatedAt,
                UserId = userTaskDTO.UserId,
                User = userTaskDTO.User
            };
        }
    }
}
