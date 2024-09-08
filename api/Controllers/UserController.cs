using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UserController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
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
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);

                if (createdUser.Succeeded) 
                {
                    var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

                    if (roleResult.Succeeded)
                        return Ok(
                            new NewUserDto
                            {
                                UserName = newUser.UserName,
                                Email = newUser.Email,
                                Token = _tokenService.CreateToken(newUser)
                            }
                        );
                    else
                        return StatusCode(500, roleResult.Errors);
                }
                
                return StatusCode(500, createdUser.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.UserName == loginDto.UserName.ToLower());

            if (user is null)
                return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("UserName not found and/or password is wrong");

            return Ok(
                new NewUserDto
                {
                    UserName = loginDto.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }
    }
}
