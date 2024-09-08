using TaskManagementSystem.Models;
using TaskManagementSystem.DTOs;

namespace TaskManagementSystem.Mappers
{
    public static class UserTaskMappers
    {
        public static UserTaskDto ToUserTaskDTO(this UserTask userTask)
        {
            return new UserTaskDto
            {
                Id = userTask.Id,
                Title = userTask.Title,
                Description = userTask.Description,
                DueDate = userTask.DueDate,
                Status = userTask.Status,
                Priority = userTask.Priority,
                CreatedAt = userTask.CreatedAt,
                UpdatedAt = userTask.UpdatedAt,
                UserId = userTask.UserId,
            };
        }

        public static UserTask ToUserTask(this CreateUserTaskRequestDto userTaskDTO)
        {
            return new UserTask
            {
                Id = Guid.NewGuid(),
                Title = userTaskDTO.Title,
                Description = userTaskDTO.Description,
                DueDate = userTaskDTO.DueDate,
                Status = userTaskDTO.Status,
                Priority = userTaskDTO.Priority,
                UserId = userTaskDTO.UserId,
            };
        }

        public static UserTask ToUserTask(this UpdateUserTaskRequestDto userTaskDTO, Guid id)
        {
            return new UserTask
            {
                Id = id,
                Title = userTaskDTO.Title,
                Description = userTaskDTO.Description,
                DueDate = userTaskDTO.DueDate,
                Status = userTaskDTO.Status,
                Priority = userTaskDTO.Priority,
                CreatedAt = userTaskDTO.CreatedAt,
                UpdatedAt = userTaskDTO.UpdatedAt,
                UserId = userTaskDTO.UserId,
            };
        }
    }
}
