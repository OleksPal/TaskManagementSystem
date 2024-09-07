using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
