using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newUser = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    CreatedAt = DateTime.Now
                };

                var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);

                if (createdUser.Succeeded) 
                {
                    var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation($"The user called {newUser.UserName} has been successfully created");

                        return Ok(
                            new NewUserDto
                            {
                                UserName = newUser.UserName,
                                Email = newUser.Email,
                                Token = _tokenService.CreateToken(newUser)
                            }
                        );
                    }

                    _logger.LogInformation($"The attempt to add to role user called {newUser.UserName} has been failed");

                    return StatusCode(500, roleResult.Errors);
                }

                _logger.LogInformation($"The attempt to create user called {newUser.UserName} has been failed");

                return StatusCode(500, createdUser.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.UserName == loginDto.UserName.ToLower());

            if (user is null)
                return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Failed login attempt from a user called {user.UserName}");
                return Unauthorized("UserName not found and/or password is wrong");
            }                

            _logger.LogInformation($"The user called {user.UserName} has successfully logged in");
            return Ok(
                new NewUserDto
                {
                    UserName = loginDto.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = HttpContext.User.FindFirstValue("username");
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return StatusCode(500);

            user.UpdatedAt = DateTime.Now;

            var passwordChangeResult = 
                await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (passwordChangeResult.Succeeded)
            {
                _logger.LogWarning($"Password of the user called {user.UserName} has been succesfully changed");
                return Ok("Password changed succesfully");
            }

            return StatusCode(500, passwordChangeResult.Errors);
        }
    }
}
