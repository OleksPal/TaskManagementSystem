using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class UserTask
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

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

        public User? User { get; set; }
    }
}
