using TaskManagementSystem.Models;

namespace TaskManagementSystem.Helpers
{
    public class QueryObject
    {
        public Status? Status { get; set; } = null;
        public DateTime? DueDate { get; set; } = null;
        public Priority? Priority { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}
