using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
