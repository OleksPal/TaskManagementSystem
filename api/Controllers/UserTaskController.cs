using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserTaskController(IUserTaskService userTaskService, UserManager<User> userManager, ILogger<UserController> logger)
        {
            _userTaskService = userTaskService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] QueryObject query)
        {
            var userName = User.GetUserName();
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return StatusCode(500, "No such user");

            var taskDtoList = await _userTaskService.GetAllTasksAsync(user.Id, query);

            if (taskDtoList is null)
                return NotFound();

            return Ok(taskDtoList);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTask([FromRoute] Guid id)
        {
            var taskDto = await _userTaskService.GetTaskByIdAsync(id);

            if (taskDto is null)
                return NotFound();

            return Ok(taskDto);                
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateUserTaskRequestDto createTaskDto)
        {
            var taskDto = await _userTaskService.AddTaskAsync(createTaskDto);

            _logger.LogInformation($"Task with id {taskDto.Id} has been created");

            return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id }, taskDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateUserTaskRequestDto updateTaskDto)
        {
            var taskDto = await _userTaskService.EditTaskAsync(id, updateTaskDto);

            if (taskDto is null)
                return NotFound();

            _logger.LogWarning($"Task with id {taskDto.Id} has been updated");

            return Ok(taskDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            var task = await _userTaskService.DeleteTaskAsync(id);

            if (task is null)
                return NotFound();

            _logger.LogWarning($"Task with id {task.Id} has been deleted");

            return NoContent();
        }
    }
}
