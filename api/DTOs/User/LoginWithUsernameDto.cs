using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.User
{
    public class LoginWithUsernameDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
