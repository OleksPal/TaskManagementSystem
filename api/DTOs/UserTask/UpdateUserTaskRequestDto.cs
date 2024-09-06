using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.DTOs
{
    public class UpdateUserTaskRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Title cannot be empty")]
        [MaxLength(100, ErrorMessage = "Title cannot be over 100 characters")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot be over 1000 characters")]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public Priority Priority { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public Guid? UserId { get; set; }
    }
}
