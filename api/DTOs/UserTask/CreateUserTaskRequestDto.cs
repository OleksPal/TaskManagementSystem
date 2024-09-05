using TaskManagementSystem.Models;

namespace TaskManagementSystem.DTOs
{
    public class CreateUserTaskRequestDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? UserId { get; set; }
    }
}
