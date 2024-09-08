using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.User
{
    public class LoginWithEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
